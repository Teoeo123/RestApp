from DBengine import *
from APIclasses import *
from DBclasses import *
from CryptoFun import *
from fastapi import File, UploadFile
from sqlalchemy.orm import Session, joinedload
from sqlalchemy import DateTime, desc
from typing import Optional, Tuple

def InsertUser(user: ApiUser):
    salt = GenSalt(10);
    session.add(DBUser(user.email, GenHash(user.passwd + salt), salt))
    session.commit()

def GetUserByMail(userMail: str) -> DBUser:
    return session.query(DBUser).filter(DBUser.email == userMail).first()

def InsertPhoto(file: UploadFile = File(...)) -> int:
    photo = DBPhoto(photo_name = file.filename, photo_data = file.file.read())
    session.add(photo)
    session.commit()
    session.refresh(photo) 
    return photo.id

def PhotoById(id: int) -> DBPhoto:
   return session.query(DBPhoto).filter(DBPhoto.id == id).first()

def GetUserDetailByUserID(id: int) -> Optional[Tuple[DBUserDetails, Optional[DBPhoto]]]:
    query = session.query(
        DBUserDetails, DBPhoto
    ).outerjoin(
        DBPhoto, DBUserDetails.profile_picture_id == DBPhoto.id
    ).filter(DBUserDetails.user_id == id)
    return query.first()


def GetUserLightDetailByUserID(id: int) -> DBUserDetails:
    query = session.query(DBUserDetails).filter(DBUserDetails.user_id == id)
    return query.first()

def GetUserDetailByID(id: int) -> DBUserDetails:
    query = session.query(DBUserDetails).filter(DBUserDetails.id == id)
    return query.first()

def InsertUserDetals(user: ApiUser, details: ApiUserDetails):
    dbUser = GetUserByMail(user.email)
    ChUser = GetUserDetailByUserID(user.id)
    
    if ChUser is None:
        detail = DBUserDetails(user_id = dbUser.id, 
                               nickname = details.nickName,
                               bio = details.bio,
                               profile_picture_id = details.profpicID)
        session.add(detail)
        session.commit()
    else:
        uprecord = GetUserLightDetailByUserID(user.id)
        if not details.nickName is None:
            uprecord.nickname = details.nickName
        if not details.bio is None:
            uprecord.bio = details.bio
        if not details.profpicID is None:
            uprecord.profile_picture_id = details.profpicID
        session.commit()

def InsertPost(user: DBUserDetails, content: str, photoId: int):
    post = DBPost(user_id = user.id, content = content, photo_id = photoId)
    session.add(post)
    session.commit()
            
def GetPostFrom(date: DateTime, count: int):
    if date is None or date == '':
        date = func.now()
    
    query = (
        session.query(DBPost)
        .filter(DBPost.date_added < date)
        .order_by(desc(DBPost.date_added))
        .limit(count)  
    )
    posts = []
    for post in query.all():  # Iterate through the query result
        user_details = GetUserDetailByID(post.user_id)
        api_post = ApiPostBack(
            id=post.id,
            user_nick=user_details.nickname,
            user_photo_id=user_details.profile_picture_id,
            photo_id=post.photo_id,
            content=post.content,
            date_added=post.date_added
        )
        posts.append(api_post)
    return posts
