CREATE VIEW CurrentInventory AS
SELECT
    p.ProductCode,
    p.ProductName,
    p.AgeGroup,
    c.ColorName,
    SUM(pc.Quantity) AS CurrentStock,
    s.StoreroomName
FROM
    Product p
JOIN
    Product_Color pc ON p.ProductCode = pc.ProductID
JOIN
    Color c ON pc.ColorID = c.ColorID
JOIN
    Storeroom s ON p.StoreroomID = s.StoreroomID
GROUP BY
    p.ProductCode, p.ProductName, p.AgeGroup, c.ColorName, s.StoreroomName;

CREATE VIEW SalesReport AS
SELECT
    s.SaleID,
    s.Date,
    p.ProductName,
    s.Quantity,
    s.TotalPrice,
    s.Discount,
    s.NetAmount,
    st.StoreroomName
FROM
    Sale s
JOIN
    Product p ON s.ProductID = p.ProductCode
JOIN
    Storeroom st ON p.StoreroomID = st.StoreroomID;

CREATE VIEW ProductStockSummary AS
SELECT 
    p.ProductCode,
    p.ProductName,
    SUM(pc.Quantity) AS TotalStockLevel,
    COUNT(pc.ColorID) AS ColorVariants
FROM 
    Product p
JOIN 
    Product_Color pc ON p.ProductCode = pc.ProductID
GROUP BY 
    p.ProductCode, p.ProductName;

CREATE VIEW SalesOverview AS
SELECT 
    p.ProductCode,
    p.ProductName,
    SUM(s.Quantity) AS TotalSold,
    SUM(s.TotalPrice) AS TotalSalesValue,
    SUM(s.Discount) AS TotalDiscount
FROM 
    Product p
JOIN 
    Sale s ON p.ProductCode = s.ProductID
GROUP BY 
    p.ProductCode, p.ProductName;

CREATE VIEW LowStockProducts AS
SELECT 
    p.ProductCode,
    p.ProductName,
    SUM(pc.Quantity) AS TotalStockLevel,
    (CASE 
        WHEN SUM(pc.Quantity) < 10 THEN 'Low'
        ELSE 'Sufficient'
    END) AS StockStatus
FROM 
    Product p
JOIN 
    Product_Color pc ON p.ProductCode = pc.ProductID
GROUP BY 
    p.ProductCode, p.ProductName
HAVING 
    SUM(pc.Quantity) < 10;