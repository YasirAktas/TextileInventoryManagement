-- Creating Storeroom Table
CREATE TABLE Storeroom (
    StoreroomID INT PRIMARY KEY, -- Unique identifier for each storeroom
    StoreroomName NVARCHAR(255) NOT NULL -- Descriptive name of the storeroom
);

-- Creating Product Table
CREATE TABLE Product (
    ProductCode INT PRIMARY KEY, -- Unique identifier for each product
    ProductName NVARCHAR(255) NOT NULL, -- Name of the product
    AgeGroup NVARCHAR(50), -- Age group the product is designed for
    UnitPrice DECIMAL(10, 2) NOT NULL, -- Price per unit of the product
    StockLevel INT NOT NULL, -- Current stock level of the product
    TotalPrice DECIMAL(10, 2), -- Total price of the product (calculated based on Product_Color)
    Discount DECIMAL(5, 2), -- Applicable discount percentage
    StockStatus NVARCHAR(50), -- Indicates if the product needs restocking
    StoreroomID INT, -- Foreign key linking to Storeroom table
    FOREIGN KEY (StoreroomID) REFERENCES Storeroom(StoreroomID) -- Ensures StoreroomID matches an existing storeroom
);

-- Creating Color Table
CREATE TABLE Color (
    ColorID INT PRIMARY KEY, -- Unique identifier for each color
    ColorName NVARCHAR(50) NOT NULL -- Name of the color (e.g., Blue, Black)
);

-- Creating Product_Color Table
CREATE TABLE Product_Color (
    ProductColorID INT PRIMARY KEY, -- Unique identifier for each product-color relationship
    ProductID INT, -- Foreign key linking to the Product table
    ColorID INT, -- Foreign key linking to the Color table
    Quantity INT NOT NULL, -- Number of units available for this specific color variant
    FOREIGN KEY (ProductID) REFERENCES Product(ProductCode), -- Ensures ProductID matches an existing product
    FOREIGN KEY (ColorID) REFERENCES Color(ColorID) -- Ensures ColorID matches an existing color
);

-- Creating InventoryTransaction Table
CREATE TABLE InventoryTransaction (
    TransactionID INT PRIMARY KEY, -- Unique identifier for each transaction
    Date DATE NOT NULL, -- Date when the transaction occurred
    TransactionType NVARCHAR(50) NOT NULL, -- Type of transaction (e.g., Add, Remove)
    Quantity INT NOT NULL, -- Number of units added or removed
    ProductID INT, -- Foreign key linking to the Product table
    StoreroomID INT, -- Foreign key linking to the Storeroom table
    FOREIGN KEY (ProductID) REFERENCES Product(ProductCode), -- Ensures ProductID matches an existing product
    FOREIGN KEY (StoreroomID) REFERENCES Storeroom(StoreroomID) -- Ensures StoreroomID matches an existing storeroom
);

-- Creating Sale Table
CREATE TABLE Sale (
    SaleID INT PRIMARY KEY, -- Unique identifier for each sale
    Date DATE NOT NULL, -- Date when the sale occurred
    Quantity INT NOT NULL, -- Number of units sold
    TotalPrice DECIMAL(10, 2) NOT NULL, -- Total price before applying the discount
    Discount DECIMAL(5, 2), -- Discount applied to the sale
    NetAmount DECIMAL(10, 2) NOT NULL, -- Final amount after discount
    ProductID INT, -- Foreign key linking to the Product table
    FOREIGN KEY (ProductID) REFERENCES Product(ProductCode) -- Ensures ProductID matches an existing product
);

