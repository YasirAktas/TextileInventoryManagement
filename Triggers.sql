CREATE TRIGGER trg_UpdateStockLevel
ON InventoryTransaction
AFTER INSERT, UPDATE
AS
BEGIN
    DECLARE @ProductID INT, @StoreroomID INT, @Quantity INT, @TransactionType NVARCHAR(50);
    
    -- Get the inserted values
    SELECT @ProductID = ProductID, @StoreroomID = StoreroomID, @Quantity = Quantity, @TransactionType = TransactionType
    FROM inserted;
    
    -- Update stock levels based on transaction type
    IF @TransactionType = 'Add'
    BEGIN
        UPDATE pc
        SET pc.Quantity = pc.Quantity + @Quantity
        FROM Product_Color pc
        INNER JOIN InventoryTransaction it ON it.ProductID = pc.ProductID
        WHERE it.ProductID = @ProductID AND it.StoreroomID = @StoreroomID;
    END
    ELSE IF @TransactionType = 'Remove'
    BEGIN
        UPDATE pc
        SET pc.Quantity = pc.Quantity - @Quantity
        FROM Product_Color pc
        INNER JOIN InventoryTransaction it ON it.ProductID = pc.ProductID
        WHERE it.ProductID = @ProductID AND it.StoreroomID = @StoreroomID;
    END
END;


CREATE TRIGGER trg_UpdateStockLevelOnSale
ON Sale
AFTER INSERT
AS
BEGIN
    DECLARE @ProductColorID INT, @QuantitySold INT;
    
    -- Get the inserted sale details
    SELECT @ProductColorID = ProductColorID, @QuantitySold = Quantity
    FROM inserted;
    
    -- Update stock level in Product_Color table
    UPDATE pc
    SET pc.Quantity = pc.Quantity - @QuantitySold
    FROM Product_Color pc
    WHERE pc.ProductColorID = @ProductColorID;
END;
