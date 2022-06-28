/* Disable foreign keys */
PRAGMA foreign_keys = 'off';

/* Begin transaction */
BEGIN;

/* Database properties */
PRAGMA auto_vacuum = 0;
PRAGMA encoding = 'UTF-8';
PRAGMA page_size = 4096;

/* Drop table [!CreationHistory] */
DROP TABLE IF EXISTS [main].[!CreationHistory];

/* Table structure [!CreationHistory] */
CREATE TABLE [main].[!CreationHistory](
  [id] INTEGER PRIMARY KEY AUTOINCREMENT, 
  [Action] TEXT, 
  [Description] TEXT);

/* Drop table [Messages] */
DROP TABLE IF EXISTS [main].[Messages];

/* Table structure [Messages] */
CREATE TABLE [main].[Messages](
  [id] INTEGER PRIMARY KEY AUTOINCREMENT, 
  [email] TEXT, 
  [subject] TEXT, 
  [message] TEXT);
CREATE INDEX [main].[MessagesIndex]
ON [Messages](
  [id], 
  [email]);

/* Drop table [MessagesAttachments] */
DROP TABLE IF EXISTS [main].[MessagesAttachments];

/* Table structure [MessagesAttachments] */
CREATE TABLE [main].[MessagesAttachments](
  [id] INTEGER PRIMARY KEY AUTOINCREMENT, 
  [MessageID] INTEGER REFERENCES [Messages]([id]) ON DELETE CASCADE, 
  [filename] TEXT, 
  [data] BLOB);
CREATE INDEX [main].[AttachmentsIndex]
ON [MessagesAttachments](
  [id], 
  [filename]);

/* Empty table [!CreationHistory] */
DELETE FROM
  [main].[!CreationHistory];

/* Table data [!CreationHistory] Record count: 6 */
INSERT INTO [!CreationHistory]([rowid], [id], [Action], [Description]) VALUES(1, 1, 'Create this table', 'to show creation history');
INSERT INTO [!CreationHistory]([rowid], [id], [Action], [Description]) VALUES(2, 2, 'Create table Messages', 'to save messages');
INSERT INTO [!CreationHistory]([rowid], [id], [Action], [Description]) VALUES(3, 3, 'Create table MessagesAttachments', 'to save attachments for messages');
INSERT INTO [!CreationHistory]([rowid], [id], [Action], [Description]) VALUES(4, 4, 'Create foreign key for Attachments', 'attachments for messages');
INSERT INTO [!CreationHistory]([rowid], [id], [Action], [Description]) VALUES(5, 5, 'Create index for Messages', 'to accelerate select');
INSERT INTO [!CreationHistory]([rowid], [id], [Action], [Description]) VALUES(6, 6, 'Create indes for attachments', 'to accelerate select');

/* Empty table [Messages] */
DELETE FROM
  [main].[Messages];

/* Empty table [MessagesAttachments] */
DELETE FROM
  [main].[MessagesAttachments];

/* Commit transaction */
COMMIT;

/* Enable foreign keys */
PRAGMA foreign_keys = 'on';
