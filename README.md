# Database Setup - City Seeding

## Required: Initial Cities Setup

After running database migrations, you need to seed cities for testing.

### Connection Details
- **Server**: `(localdb)\mssqllocaldb`
- **Authentication**: Windows Authentication
- **Database**: `PersonDirectoryDb`

### City Seeding Script

Open SQL Server Management Studio and run this script:

```sql
INSERT INTO Cities (Name, CreatedAt, UpdatedAt) VALUES
	('Tbilisi', GETUTCDATE(), GETUTCDATE()),
	('Oslo', GETUTCDATE(), GETUTCDATE()),
	('Batumi', GETUTCDATE(), GETUTCDATE()),
	('Porto', GETUTCDATE(), GETUTCDATE()),
	('Palermo', GETUTCDATE(), GETUTCDATE());

-- Verify insertion
SELECT Id, Name FROM Cities ORDER BY Id;
```
