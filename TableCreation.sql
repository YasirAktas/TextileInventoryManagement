-- Create the Storeroom table
CREATE TABLE Storeroom (
    StoreroomID INT PRIMARY KEY IDENTITY(1,1), -- Auto-increment unique identifier
    StoreroomName NVARCHAR(100) NOT NULL
);
-- Create the Product table
CREATE TABLE Product (
    ProductCode INT PRIMARY KEY IDENTITY(1,1),  -- Auto-increment unique identifier
    ProductName NVARCHAR(100) NOT NULL,
    AgeGroup NVARCHAR(50) NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    StockLevel INT NOT NULL DEFAULT 0,
    StockStatus AS (CASE WHEN StockLevel < 10 THEN 'Low' ELSE 'Sufficient' END) PERSISTED,  -- Computed column
    Discount DECIMAL(5,2) CHECK (Discount BETWEEN 0 AND 100),
    StoreroomID INT NOT NULL,  -- Foreign key referencing Storeroom
    CONSTRAINT FK_Product_Storeroom FOREIGN KEY (StoreroomID) REFERENCES Storeroom(StoreroomID) ON DELETE NO ACTION ON UPDATE CASCADE  -- Cascade on update, no action on delete
);

-- Create the Color table
CREATE TABLE Color (
    ColorID INT PRIMARY KEY IDENTITY(1,1), -- Auto-increment unique identifier
    ColorName NVARCHAR(50) NOT NULL
);

-- Create the Product_Color table (junction table)
CREATE TABLE Product_Color (
    ProductColorID INT PRIMARY KEY IDENTITY(1,1), -- Auto-increment unique identifier
    ProductCode INT NOT NULL, -- Foreign key
    ColorID INT NOT NULL, -- Foreign key
    Quantity INT NOT NULL CHECK (Quantity >= 0), -- Quantity validation
    PricePerUnit DECIMAL(10,2) NOT NULL CHECK (PricePerUnit >= 0), -- Positive pricing
    TotalPrice AS (Quantity * PricePerUnit) PERSISTED, -- Computed column
    FOREIGN KEY (ProductCode) REFERENCES Product(ProductCode) ON DELETE CASCADE ON UPDATE CASCADE,  -- Delete or update cascade
    FOREIGN KEY (ColorID) REFERENCES Color(ColorID) ON DELETE CASCADE ON UPDATE CASCADE  -- Delete or update cascade
);



CREATE TABLE InventoryTransaction (
    TransactionID INT PRIMARY KEY IDENTITY(1,1),  -- Auto-increment unique identifier
    Date DATETIME NOT NULL,                         -- Date of the transaction
    TransactionType NVARCHAR(50) CHECK (TransactionType IN ('Add', 'Remove')),  -- Type of transaction
    Quantity INT NOT NULL CHECK (Quantity >= 0),    -- Quantity of items involved in the transaction
    ProductID INT NOT NULL,                         -- Foreign key referencing Product
    StoreroomID INT NOT NULL,                       -- Foreign key referencing Storeroom
    FOREIGN KEY (ProductID) REFERENCES Product(ProductCode),
    FOREIGN KEY (StoreroomID) REFERENCES Storeroom(StoreroomID)
);


CREATE TABLE Sale (
    SaleID INT PRIMARY KEY IDENTITY(1,1),          -- Auto-increment unique identifier
    ProductColorID INT NOT NULL,                    -- Foreign key referencing Product_Color table
    Quantity INT NOT NULL CHECK (Quantity > 0),     -- Quantity of items sold (must be positive)
    SaleDate DATETIME NOT NULL,                     -- Date of sale
    FOREIGN KEY (ProductColorID) REFERENCES Product_Color(ProductColorID) 
    ON DELETE CASCADE ON UPDATE CASCADE            -- Delete or update cascade
);

-- Adding foreign key constraint for ProductID referencing Product(ProductCode)
ALTER TABLE InventoryTransaction
ADD CONSTRAINT FK_InventoryTransaction_Product
FOREIGN KEY (ProductID) REFERENCES Product(ProductCode)
    ON DELETE NO ACTION  -- Prevent deletion of rows in InventoryTransaction if Product is deleted
    ON UPDATE CASCADE;   -- Update ProductID if ProductCode is updated in the Product table

-- Add foreign key constraint for StoreroomID referencing Storeroom(StoreroomID)
ALTER TABLE InventoryTransaction
ADD CONSTRAINT FK_InventoryTransaction_Storeroom
FOREIGN KEY (StoreroomID) REFERENCES Storeroom(StoreroomID)
    ON DELETE NO ACTION    -- Avoid cascading delete
    ON UPDATE NO ACTION;   -- No cascading updates for this FK

