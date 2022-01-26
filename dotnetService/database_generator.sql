.open RECORD.db

CREATE TABLE MessageStatus (
    MessageID TEXT PRIMARY KEY,
    DialogueID INTEGER,
    TIME_RECORD TEXT, 
    KIND TEXT,
    MessageText TEXT,
    ReadStatus TEXT
);
--Refer
--https://stackoverflow.com/questions/5299267/how-to-create-enum-type-in-sqlite

CREATE TABLE USERS (
    ID INTEGER PRIMARY KEY ,
    UserName TEXT,
    Phone TEXT, 
    UserGroup TEXT 
);

INSERT INTO USERS VALUES (10,"Temp Surname", "+918101", "Z");

CREATE TABLE SessionStatus (
    SessionID TEXT,
    UserID INTEGER,
    Category INTEGER, 
    Stage INTEGER,
    DialogueID INTEGER
);