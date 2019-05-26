use InternetMarket
go

Insert into Roles values
('Admin'),
('Customer')
-- Insert admins only, customer for an example
insert into Users values
('Admin', '12345678', 'Admin'),
('Customer', '123456', 'Customer')

insert into Status values
('Processing'),
('In transit'),
('Completed'),
('Canceled')

insert into Categories values
('Kitchen'),
('Sport'),
('House'),
('Other')

insert into Products values
-- Kitchen products
('Silver spoon', 'Just spoon', 'Kitchen', 0, 0, 9.99),
('Gold spoon', 'Just spoon', 'Kitchen', 5, 1, 19.99),
('Bronze fork', 'Just fork', 'Kitchen', 0, 0, 3.99),
('Small kettle', 'Simple kettle', 'Kitchen', 1, 1, 20),
('Small plate', 'Plate', 'Kitchen', 0, 0, 9.99),
-- Sport products
('30lb dumbbells', 'small dumbbells', 'Sport', 0, 0, 19.99),
('60lb dumbbells', 'medium dumbbells', 'Sport', 3, 1, 29.99),
('100lb dumbbells', 'huge dumbbells', 'Sport', 5, 1, 39.99),
('20kg bar', 'just bar', 'Sport', 0, 0, 9.99),
('Small ball', 'Football stuff', 'Sport', 0, 0, 4.99),
-- House products
('Red broom', 'just broom', 'House', 4, 1, 3.99),
('Yellow broom', 'just broom', 'House', 0, 0, 3.99),
('Small green carpet', 'Carpet', 'House', 0, 0, 2.99),
('Big red carpet', 'Carpet', 'House', 0, 0, 4.99),
('Big green carpet', 'Carpet', 'House', 2, 1, 4.99),
-- Other products
('Notebook LenovoA12345', 'Lenovo', 'House', 2, 1, 200.99),
('Notebook LenovoA54321', 'Lenovo', 'House', 4, 1, 230.99),
('Notebook AsusB51423', 'Asus', 'House', 4, 1, 240.99),
('Notebook AsusB15243', 'Asus', 'House', 4, 1, 280.99),
('Notebook AcerC32415', 'Acer', 'House', 4, 1, 300.99)