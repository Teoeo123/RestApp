from sqlalchemy import Column, String, Integer,LargeBinary, ForeignKey, DateTime  # type: ignore
from sqlalchemy.sql import func 
from sqlalchemy.orm import relationship
from DBengine import Base


class DBUser(Base):
    __tablename__ = "users"
    id = Column("id", Integer, primary_key = True)
    email = Column("email", String, nullable = False, unique = True)
    passwd = Column("passwd",String, nullable = False)
    salt = Column("salt", String, nullable = False)

    def __init__(self, email, passwd, salt):
        self.email = email
        self.passwd = passwd
        self.salt = salt

class DBUserDetails(Base):
    __tablename__ = 'user_details'

    id = Column(Integer, primary_key=True)
    user_id = Column(Integer, ForeignKey('users.id'), nullable=False, unique = True)
    nickname = Column(String)
    bio = Column(String)
    profile_picture_id = Column(Integer, ForeignKey('photos.id'))

    # This breaking api
    # user = relationship("User", back_populates="details")
    # profile_picture = relationship("Photo", back_populates="user_details")

class DBPhoto(Base):
    __tablename__ = 'photos'
    id = Column("id",Integer, primary_key=True)
    photo_data = Column("photo",LargeBinary)
    photo_name = Column("filename",String)

class DBPost(Base):
    __tablename__ = 'post'

    id = Column(Integer, primary_key=True)
    user_id = Column(Integer, ForeignKey('user_details.id'), nullable=False)
    photo_id = Column(Integer, ForeignKey('photos.id'))
    content = Column(String)
    date_added = Column(DateTime(timezone=True), server_default=func.now()) 
