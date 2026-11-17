-- ==============================================================================
-- NETHERET IT SUPPORT SOLUTIONS
-- Database Setup Script (SQLite)
-- ==============================================================================

-- 1. CLEANUP
-- Drop existing tables if they exist so students can easily reset their database 
-- if they make a mistake during development.
DROP TABLE IF EXISTS StaffUsers;
DROP TABLE IF EXISTS Roles;

-- ==============================================================================
-- 2. SCHEMA CREATION
-- ==============================================================================

-- Create the Roles lookup table
CREATE TABLE Roles (
    RoleTierID INTEGER PRIMARY KEY AUTOINCREMENT,
    RoleName VARCHAR(50) UNIQUE NOT NULL
);

-- Create the main Users table
-- RoleTier acts as a Foreign Key linking back to the Roles table.
CREATE TABLE StaffUsers (
    UserID INTEGER PRIMARY KEY AUTOINCREMENT,
    Username VARCHAR(50) UNIQUE NOT NULL,
    PasswordHash VARCHAR(256) NOT NULL,
    RoleTier INTEGER NOT NULL,
    FOREIGN KEY (RoleTier) REFERENCES Roles(RoleTierID)
);

-- ==============================================================================
-- 3. SEED DATA (Populating the tables for testing)
-- ==============================================================================

---- Insert the standard Netheret IT support tiers
INSERT INTO Roles (RoleName) VALUES ('Tier 1 Support');
INSERT INTO Roles (RoleName) VALUES ('Tier 2 Engineer');
INSERT INTO Roles (RoleName) VALUES ('Tier 3 Administrator');

-- Insert Test Users
-- SECURITY NOTE: Passwords must NEVER be stored in plain text.
-- The hash below ('ef92b...') is the SHA256 equivalent of the word: password123
-- Students must hash their API input and compare it to this string.

--INSERT INTO StaffUsers (Username, PasswordHash, RoleTier)
--VALUES (
--    'jsmith', 
--    'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 
--    1
--);

--INSERT INTO StaffUsers (Username, PasswordHash, RoleTier)
--VALUES (
--    'abrown', 
--    'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 
--    3
--);

-- Note for SQL Server users: 
-- If you choose to use Microsoft SQL Server instead of SQLite, change:
-- "INTEGER PRIMARY KEY AUTOINCREMENT" to "INT PRIMARY KEY IDENTITY(1,1)"