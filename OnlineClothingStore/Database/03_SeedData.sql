SET IDENTITY_INSERT UserRole ON;
INSERT INTO UserRole (Id, Name) VALUES
(1, 'Admin'),
(2, 'Customer')
SET IDENTITY_INSERT UserRole OFF;

SET IDENTITY_INSERT OrderStatus ON;
INSERT INTO OrderStatus (Name) VALUES
(1, 'Processing'),
(2, 'Shipped'),
(3, 'Delivered'),
(4, 'Cancelled');
SET IDENTITY_INSERT OrderStatus OFF;

SET IDENTITY_INSERT InventoryLogChangeType ON;
INSERT INTO InventoryLogChangeType (Name) VALUES
(1, 'Sale'),
(2, 'Restock'),
(3, 'Adjustment'),
(4, 'Return'),
(5, 'Damage');
SET IDENTITY_INSERT InventoryLogChangeType OFF;