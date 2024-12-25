CREATE PROCEDURE AddNewProduct
    @ProductCode INT,
    @ProductName NVARCHAR(255),
    @UnitPrice DECIMAL(10,2),
    @ColorID INT,
    @InitialQuantity INT,
    @PricePerUnit DECIMAL(10,2)
AS
BEGIN
    -- Insert a new product into the Product table
    INSERT INTO Product (ProductCode, ProductName, UnitPrice)
    VALUES (@ProductCode, @ProductName, @UnitPrice);

    -- Insert a new product-color record into the Product_Color table
    INSERT INTO Product_Color (ProductCode, ColorID, Quantity, PricePerUnit)
    VALUES (@ProductCode, @ColorID, @InitialQuantity, @PricePerUnit);

    -- Output the result (optional)
    SELECT 'Product added successfully' AS Message;
END;

CREATE PROCEDURE RecordSale
    @ProductColorID INT,
    @Quantity INT,
    @SaleDate DATETIME
AS
BEGIN
    -- Insert the sale record into the Sale table
    INSERT INTO Sale (ProductColorID, Quantity, SaleDate)
    VALUES (@ProductColorID, @Quantity, @SaleDate);

    -- Output the result (optional)
    SELECT 'Sale recorded successfully' AS Message;
END;

CREATE PROCEDURE UpdateProductPrice
    @ProductColorID INT,
    @NewPrice DECIMAL(10,2)
AS
BEGIN
    -- Update the price of the specified product-color combination
    UPDATE Product_Color
    SET PricePerUnit = @NewPrice
    WHERE ProductColorID = @ProductColorID;

    -- Output the result (optional)
    SELECT 'Price updated successfully' AS Message;
END;

