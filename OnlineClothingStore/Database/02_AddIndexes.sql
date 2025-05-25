USE OnlineClothingStore;
GO

-- USER Table
CREATE NONCLUSTERED INDEX IX_User_RoleId
ON [User] (RoleId);

-- Category Table
CREATE NONCLUSTERED INDEX IX_Category_ParentCategoryId
ON Category (ParentCategoryId);

-- Product Table
CREATE NONCLUSTERED INDEX IX_Product_CategoryId
ON [Product] (CategoryId);

CREATE NONCLUSTERED INDEX IX_Product_BrandId
ON [Product] (BrandId);

CREATE NONCLUSTERED INDEX IX_Product_Price
ON [Product] (Price);

CREATE NONCLUSTERED INDEX IX_Product_Name
ON [Product] ([Name]);

CREATE NONCLUSTERED INDEX IX_Product_CategoryId_BrandId
ON [Product] (CategoryId, BrandId)
INCLUDE ([Name], Price);

-- ProductVariant Table
CREATE NONCLUSTERED INDEX IX_ProductVariant_ProductId
ON [ProductVariant] (ProductId);

CREATE NONCLUSTERED INDEX IX_ProductVariant_StockQuantity
ON ProductVariant (StockQuantity);

-- CartItem Table
CREATE NONCLUSTERED INDEX IX_CartItem_CartId
ON CartItem (CartId);

CREATE NONCLUSTERED INDEX IX_CartItem_ProductVariantId
ON CartItem (ProductVariantId);

-- Order Table
CREATE NONCLUSTERED INDEX IX_Order_UserId
ON [Order] (UserId);

CREATE NONCLUSTERED INDEX IX_Order_OrderStatusId
ON [Order] (OrderStatusId);

CREATE NONCLUSTERED INDEX IX_Order_OrderDate
ON [Order] (OrderDate);

CREATE NONCLUSTERED INDEX IX_Order_TotalAmount
ON [Order] (TotalAmount);

-- OrderItem Table
CREATE NONCLUSTERED INDEX IX_OrderItem_OrderId
ON OrderItem (OrderId);

CREATE NONCLUSTERED INDEX IX_OrderItem_ProductVariantId
ON OrderItem (ProductVariantId);

-- Payment Table
CREATE NONCLUSTERED INDEX IX_Payment_PaymentDate
ON Payment (PaymentDate);

-- InventoryLog Table
CREATE NONCLUSTERED INDEX IX_InventoryLog_ProductVariantId
ON InventoryLog (ProductVariantId);

CREATE NONCLUSTERED INDEX IX_InventoryLog_ChangeTypeId
ON InventoryLog (ChangeTypeId);

CREATE NONCLUSTERED INDEX IX_InventoryLog_CreatedAt
ON InventoryLog (CreatedAt);

CREATE NONCLUSTERED INDEX IX_InventoryLog_ProductVariantId_CreatedAt
ON InventoryLog (ProductVariantId, CreatedAt);