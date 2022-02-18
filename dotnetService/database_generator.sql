--usage .\sqlite3.exe RECORD.db ".read database_generator.sql"
.open RECORD.db

CREATE TABLE MessageStatus (
    MessageID TEXT PRIMARY KEY,
    DialogueID TEXT,
    TIME_RECORD TEXT, 
    KIND TEXT,
    MessageText TEXT,
    ReadStatus TEXT
);
--Refer
--https://stackoverflow.com/questions/5299267/how-to-create-enum-type-in-sqlite

CREATE TABLE REGISTRATION (
    ID INTEGER PRIMARY KEY ,
    UserName TEXT,
    Phone TEXT, 
    UserGroup TEXT 
);

INSERT INTO REGISTRATION VALUES (10,"Temp Surname", "+11101", "Z");

CREATE TABLE SessionStatus (
    SessionID TEXT,
    UserID INTEGER,
    Category INTEGER, 
    Stage INTEGER,
    DialogueID TEXT
);

CREATE TABLE USERS (
    ID INTEGER PRIMARY KEY ,
    UserName TEXT,
    Phone TEXT, 
    UserGroup TEXT 
);

CREATE TABLE Escalation (
    ID TEXT PRIMARY KEY ,
    TIME_RECORD TEXT,
    LAST_STATUS TEXT 
);

CREATE TABLE ProductionGroup (
    SR INTEGER PRIMARY KEY , --serial number to identify line record
    ID INTEGER, --ID of the user from the USERTABLE
    ProdGroup TEXT --production group  
);