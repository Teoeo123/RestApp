from fastapi import Depends, FastAPI, HTTPException, File, UploadFile
from fastapi.security import OAuth2PasswordRequestForm
from fastapi.responses import FileResponse, StreamingResponse
from datetime import timedelta, datetime
from typing import Annotated, Tuple
from io import BytesIO
from DBclasses import *
from APIclasses import *
import DBaction as dba
import CryptoFun as cf
import sys

ACCESS_TOKEN_EXPIRE_MINUTES = 30
app = FastAPI()

@app.get("/users/me/", response_model=ApiUserDetails)
async def read_users_me(
    current_user: Annotated[ApiUser, Depends(cf.get_current_active_user)],
):
    dbUser = dba.GetUserByMail(current_user.email)
    details = dba.GetUserLightDetailByUserID(dbUser.id)
    if details is None:
        return ApiUserDetails()
    else:
        return ApiUserDetails(nickName = details.nickname,
                            bio = details.bio,
                            profpicID = details.profile_picture_id
                            )
    
@app.get("/users/photo")
async def get_user_prof_pic(photoId: int)-> StreamingResponse:
    dbphoto = dba.PhotoById(photoId)
    headers = {
        "Content-Disposition": f'attachment; filename="{dbphoto.photo_name}"',
        "Content-Type": "image/jpeg" 
    }

    return StreamingResponse(
        content=BytesIO(dbphoto.photo_data),  # Send the BLOB data as a stream
        headers=headers,
        media_type="image/jpeg",
    )

@app.get("/social/post/")
async def Get_Post(count: int, fromDate: Optional[datetime] = None):
    postlist = dba.GetPostFrom(fromDate, count)
    return postlist

@app.post("/social/post/")
async def Send_Post(current_user: Annotated[ApiUser, Depends(cf.get_current_active_user)],
                         content: Optional[str] = None,
                         file: UploadFile = Optional[File(None)]
                         ):
    isFileEmpty = False
    try:
        file.size
    except:
        isFileEmpty = True
    if not isFileEmpty: photoId = dba.InsertPhoto(file)
    else: photoId = None
    userId = dba.GetUserByMail(current_user.email).id
    detUser = dba.GetUserLightDetailByUserID(userId)
    if detUser is None:
        raise HTTPException(status_code=400, detail="Fill in your account details!")  
    dba.InsertPost(detUser, content, photoId)
    return {"detail":"Post added!"}


@app.post("/social/profil/")
async def setUserDetails(current_user: Annotated[ApiUser, Depends(cf.get_current_active_user)],
                         nick: Optional[str] = None, bio: Optional[str] = None,
                         file: UploadFile = Optional[File(None)]
                         ):
    isFileEmpty = False
    try:
        file.size
    except:
        isFileEmpty = True
    if not isFileEmpty: photoId = dba.InsertPhoto(file)
    else: photoId = None
    det = ApiUserDetails(nickName = nick, bio = bio, profpicID = photoId )
    dba.InsertUserDetals(current_user, det)
    
    return {"detail":"Details added!"}

@app.post('/auth/signup')
async def AddUser(user: ApiUser):
    if dba.GetUserByMail(user.email) is not None:
        raise HTTPException(status_code=400, detail="User already exists!")  
    dba.InsertUser(user)
    return {"detail":"User created!"}


@app.post("/token")
async def login(form_data: Annotated[OAuth2PasswordRequestForm, Depends()]) -> Token:
    findUser = dba.GetUserByMail(form_data.username)
    if findUser is None:
        raise HTTPException(status_code=400, detail="Incorrect username or password")  
    rhash = cf.GenHash(form_data.password + findUser.salt)
    if not rhash == findUser.passwd:
        raise HTTPException(status_code=400, detail="Incorrect username or password")  
    access_token_expires = timedelta(minutes=ACCESS_TOKEN_EXPIRE_MINUTES)
    access_token = cf.create_access_token(
        data={"sub": findUser.email}, expires_delta=access_token_expires
    ) 
    return Token(access_token=access_token, token_type="bearer")


