/* Disable foreign keys */
PRAGMA foreign_keys = 'off';

/* Begin transaction */
BEGIN;

/* Database properties */
PRAGMA auto_vacuum = 1;
PRAGMA encoding = 'UTF-8';
PRAGMA page_size = 4096;

/* Drop table [KeyValue] */
DROP TABLE IF EXISTS [main].[KeyValue];

/* Table structure [KeyValue] */
CREATE TABLE [main].[KeyValue](
  [key] TEXT PRIMARY KEY NOT NULL ON CONFLICT IGNORE UNIQUE ON CONFLICT IGNORE, 
  [value] TEXT
);

/* Commit transaction */
COMMIT;
