from pydantic import BaseModel
from typing import Annotated, Optional
from datetime import timedelta, datetime

class ApiUser(BaseModel):
    email: str
    passwd: Optional[str]

class Token(BaseModel):
    access_token: str
    token_type: str

class TokenData(BaseModel):
    username: str | None = None

class ApiUserDetails(BaseModel):
    nickName: str | None = None
    bio: str | None = None
    profpicID: int | None = None

class ApiPhoto(BaseModel):
    id: int
    content: bytes
    fileName: str
    
class ApiPost(BaseModel):
    id: int
    user_id: int
    photo_id: int
    content: str
    date_added: datetime

class ApiPostBack(BaseModel):
    id: int
    user_nick: Optional[str]
    user_photo_id: Optional[int]
    photo_id: Optional[int]
    content: Optional[str]
    date_added: datetime