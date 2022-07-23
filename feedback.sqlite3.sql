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

/* Drop table [Attachments] */
DROP TABLE IF EXISTS [main].[Attachments];

/* Table structure [Attachments] */
CREATE TABLE [main].[Attachments](
  [id] INTEGER PRIMARY KEY AUTOINCREMENT, 
  [filename] TEXT, 
  [data] BLOB, 
  UNIQUE([filename], [data]) ON CONFLICT IGNORE);
CREATE INDEX [main].[AttachmentsIndex] ON [Attachments]([id]);

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
  [AttachmentID] INTEGER REFERENCES [Attachments]([id]) ON DELETE CASCADE, 
  UNIQUE([MessageID], [AttachmentID]) ON CONFLICT IGNORE);
CREATE INDEX [main].[MesagesAttachmentsIndex] ON [MessagesAttachments]([id]);

/* Empty table [!CreationHistory] */
DELETE FROM
  [main].[!CreationHistory];

/* Table data [!CreationHistory] Record count: 9 */
INSERT INTO [!CreationHistory]([rowid], [id], [Action], [Description]) VALUES(1, 1, 'Create this table', 'to show creation history');
INSERT INTO [!CreationHistory]([rowid], [id], [Action], [Description]) VALUES(2, 2, 'Create table Messages', 'to save messages');
INSERT INTO [!CreationHistory]([rowid], [id], [Action], [Description]) VALUES(3, 3, 'Create table Attachments', 'to save attachments there');
INSERT INTO [!CreationHistory]([rowid], [id], [Action], [Description]) VALUES(4, 4, 'Create table MessagesAttachments', 'to attach attachments for messages');
INSERT INTO [!CreationHistory]([rowid], [id], [Action], [Description]) VALUES(5, 5, 'Create foreign key for Attachments', 'attachments for messages');
INSERT INTO [!CreationHistory]([rowid], [id], [Action], [Description]) VALUES(6, 6, 'Create foreign key for Messages', 'messages for attachments');
INSERT INTO [!CreationHistory]([rowid], [id], [Action], [Description]) VALUES(7, 7, 'Create index for Messages', 'to accelerate select');
INSERT INTO [!CreationHistory]([rowid], [id], [Action], [Description]) VALUES(8, 8, 'Create index for attachments', 'to accelerate select');
INSERT INTO [!CreationHistory]([rowid], [id], [Action], [Description]) VALUES(9, 9, 'Create index for MessagesAttachments', 'to accelerate select');

/* Empty table [Attachments] */
DELETE FROM
  [main].[Attachments];

/* Table data [Attachments] Record count: 0 */

/* Empty table [Messages] */
DELETE FROM
  [main].[Messages];

/* Table data [Messages] Record count: 0 */

/* Empty table [MessagesAttachments] */
DELETE FROM
  [main].[MessagesAttachments];

/* Table data [MessagesAttachments] Record count: 0 */

/* Commit transaction */
COMMIT;

/* Enable foreign keys */
PRAGMA foreign_keys = 'on';
