CREATE TABLE products (
	id INT PRIMARY KEY IDENTITY,
	name VARCHAR(50) NOT NULL,
	price INT NOT NULL,
	quantity INT NOT NULL
);

CREATE TABLE transaction_details (
	transaction_id VARCHAR(256) NOT NULL,
	product_id INT NOT NULL,
	quantity INT NOT NULL,
);

CREATE TABLE transactions (
	id VARCHAR(256) NOT NULL,
	method VARCHAR(50) NOT NULL
);
