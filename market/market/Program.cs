using System;
using System.Data;
using System.Data.SqlClient;

namespace market
{
    class Program
    {   
        // function for the user
        static void AsUser()
        {
            Console.WriteLine("1. View Product");
            Console.WriteLine("2. Buy Product");
            Console.WriteLine("3. Exit");
            Console.WriteLine("Choice:");
            string choice1 = Console.ReadLine();
            if (choice1 == "1")
            {
                SqlConnection conn = new SqlConnection(@"Data Source=.;Initial Catalog=marketDB;Integrated Security=True");
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from products";
                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    Console.WriteLine("View Product");
                    Console.WriteLine("============");
                    while (rdr.Read())
                    {
                        Console.WriteLine("Product ID       : {0}", rdr["id"]);
                        Console.WriteLine("Product Name     : {0}", rdr["name"]);
                        Console.WriteLine("Product Quantity : {0:#,0}", rdr["quantity"]);
                        Console.WriteLine("Product Price    : Rp {0:#,0}", rdr["price"]);
                        Console.WriteLine();
                    }
                    conn.Close();
                    Main();
                } else
                {
                    Console.WriteLine("No product available!");
                }
            }
            if (choice1 == "2")
            {
                Console.WriteLine("Buy Product");
                Console.WriteLine("===========");
                // generate unique id
                string transaction_id = Guid.NewGuid().ToString();
                int grand_total = 0;
                void Buy()
                {
                    // input product id
                    int product_id = 0;
                    void InputProductId()
                    {
                        Console.WriteLine("Input Product Id [1-9] : ");
                        product_id = int.Parse(Console.ReadLine());
                    }
                    InputProductId();
                    if (product_id < 1 || product_id > 9)
                    {
                        InputProductId();
                    }
                    // check availability
                    SqlConnection conn = new SqlConnection(@"Data Source=.;Initial Catalog=marketDB;Integrated Security=True");
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "select * from products where id=" + product_id + "";
                    conn.Open();
                    SqlDataReader read_price = cmd.ExecuteReader();
                    if(read_price.Read() == false)
                    {
                        Console.WriteLine("No product available!");
                        InputProductId();
                    }
                    conn.Close();
                    // input quantity
                    int qty = 0;
                    void InputQty()
                    {
                        Console.WriteLine("Input Product Quantity [1-20]: ");
                        qty = int.Parse(Console.ReadLine());
                    }
                    InputQty();
                    if (qty < 1 || qty > 20)
                    {
                        InputQty();
                    }
                    int current_qty = 0;
                    // compute price
                    void CompPrice()
                    {
                        SqlConnection conn = new SqlConnection(@"Data Source=.;Initial Catalog=marketDB;Integrated Security=True");
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandText = "select * from products where id=" + product_id + "";
                        conn.Open();
                        SqlDataReader read_price = cmd.ExecuteReader();
                        while (read_price.Read())
                        {
                            grand_total += (int.Parse(Convert.ToString(read_price["price"])) * qty);
                            current_qty = int.Parse(Convert.ToString(read_price["quantity"]));
                        }
                        conn.Close();
                    }
                    CompPrice();
                    // update inventory
                    void UpdateInventory()
                    {
                        SqlConnection conn = new SqlConnection(@"Data Source=.;Initial Catalog=marketDB;Integrated Security=True");
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = conn;
                        int updated_qty = current_qty - qty;
                        cmd.CommandText = "update products set quantity=" + updated_qty + " where id=" + product_id + "";
                        conn.Open();
                        cmd.ExecuteReader();
                        conn.Close();
                    }
                    UpdateInventory();
                    // insert transaction details
                    void InsertTransactionDetails()
                    {
                        SqlConnection conn = new SqlConnection(@"Data Source=.;Initial Catalog=marketDB;Integrated Security=True");
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandText = "insert into transaction_details(transaction_id,product_id,quantity) values ('" + transaction_id + "'," + product_id + ",'"+qty+"')";
                        conn.Open();
                        cmd.ExecuteReader();
                        conn.Close();
                    }
                    InsertTransactionDetails();
                    // ask for another product selection
                    Console.WriteLine("Do you want to add another product? [Yes | No]: ");
                    string decide = Console.ReadLine();
                    if (decide == "Yes")
                    {
                        Buy();
                    } else
                    {
                        Console.WriteLine("Choose payment method [Cash | Credit]: ");
                        string payment_method = Console.ReadLine();
                        if (payment_method == "Cash")
                        {
                            Console.WriteLine("Rp {0:#,0} Successfully paid by Cash!", grand_total);
                        } else
                        {
                            Console.WriteLine("Rp {0:#,0} Successfully paid by Credit!", grand_total);
                        }
                        
                        void InsertTransaction()
                        {
                            SqlConnection conn = new SqlConnection(@"Data Source=.;Initial Catalog=marketDB;Integrated Security=True");
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = conn;
                            cmd.CommandText = "insert into transactions(id,method) values ('"+ transaction_id + "','"+payment_method+"')";
                            conn.Open();
                            cmd.ExecuteReader();
                            conn.Close();
                        }
                        InsertTransaction();
                        AsUser();
                        Main();
                    }
                }
                Buy();
            }
            if(choice1 == "3")
            {
                Main();
            }
        }
        // function for the admin
        static void AsAdmin()
        {
            Console.WriteLine("1. Insert Product");
            Console.WriteLine("2. Update Product");
            Console.WriteLine("3. Delete Product");
            Console.WriteLine("4. View Product");
            Console.WriteLine("5. View Transaction");
            Console.WriteLine("6. Exit");
            Console.WriteLine("Choice:");
            string choice = Console.ReadLine();
            if(choice == "1")
            {
                Console.WriteLine("Insert Product");
                Console.WriteLine("==============");
                string name;
                void InputName()
                {
                    Console.WriteLine("Input Product Name [Length between 5-20]:");
                    name = Console.ReadLine();
                    if(name.Length < 6 || name.Length > 19)
                    {
                        InputName();
                    }
                }
                InputName();
                int price;
                void InputPrice()
                {
                    Console.WriteLine("Input Product Price [1000 - 1000000]: ");
                    price = int.Parse(Console.ReadLine());
                    if(price < 1000 || price > 1000000)
                    {
                        InputPrice();
                    }
                }
                InputPrice();
                int qty;
                void InputQty()
                {
                    Console.WriteLine("Input Product Quantity [1-1000]: ");
                    qty = int.Parse(Console.ReadLine());
                    if(qty < 1 || qty > 1000)
                    {
                        InputQty();
                    }
                }
                InputQty();
                SqlConnection conn = new SqlConnection(@"Data Source=.;Initial Catalog=marketDB;Integrated Security=True");
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "insert into products(name,price,quantity) values ('"+name+"',"+price+","+qty+")";
                conn.Open();
                cmd.ExecuteReader();
                conn.Close();
                Console.WriteLine("The product has been successfully inserted!");
                AsAdmin();
            }
            if(choice == "2")
            {
                Console.WriteLine("Update Product");
                Console.WriteLine("==============");
                int id;
                void InputId()
                {
                    Console.WriteLine("Input Product Id [1-10]:");
                    id = int.Parse(Console.ReadLine());
                    if (id < 1 || id > 10)
                    {
                        InputId();
                    }
                }
                InputId();
                string name;
                void InputName()
                {
                    Console.WriteLine("Input Product Name [Length between 5-20]:");
                    name = Console.ReadLine();
                    if (name.Length < 6 || name.Length > 19)
                    {
                        InputName();
                    }
                }
                InputName();
                int price;
                void InputPrice()
                {
                    Console.WriteLine("Input Product Price [1000 - 1000000]: ");
                    price = int.Parse(Console.ReadLine());
                    if (price < 1000 || price > 1000000)
                    {
                        InputPrice();
                    }
                }
                InputPrice();
                int qty;
                void InputQty()
                {
                    Console.WriteLine("Input Product Quantity [1-1000]: ");
                    qty = int.Parse(Console.ReadLine());
                    if (qty < 1 || qty > 1000)
                    {
                        InputQty();
                    }
                }
                InputQty();
                SqlConnection conn = new SqlConnection(@"Data Source=.;Initial Catalog=marketDB;Integrated Security=True");
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "update products set name='"+name+"',price="+price+",quantity="+qty+" where id="+id+"";
                conn.Open();
                cmd.ExecuteReader();
                conn.Close();
                Console.WriteLine("The product has been successfully updated!");
                AsAdmin();
            }
            if(choice == "3")
            {
                Console.WriteLine("Delete Product");
                Console.WriteLine("==============");
                int id;
                void InputId()
                {
                    Console.WriteLine("Input Product Id [1-10]:");
                    id = int.Parse(Console.ReadLine());
                    if (id < 1 || id > 10)
                    {
                        InputId();
                    }
                }
                InputId();
                Console.WriteLine();
                SqlConnection conn = new SqlConnection(@"Data Source=.;Initial Catalog=marketDB;Integrated Security=True");
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from products where id=" + id + "";
                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Console.WriteLine("Product ID       : {0}", rdr["id"]);
                    Console.WriteLine("Product Name     : {0}", rdr["name"]);
                    Console.WriteLine("Product Quantity : {0:#,0}", rdr["quantity"]);
                    Console.WriteLine("Product Price    : Rp {0:#,0}", rdr["price"]);
                    Console.WriteLine();
                }
                conn.Close();
                Console.WriteLine("Are you sure you want to delete this product? [Yes | No]:");
                string decision = Console.ReadLine();
                void DeleteProduct()
                {
                    SqlConnection conn = new SqlConnection(@"Data Source=.;Initial Catalog=marketDB;Integrated Security=True");
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "delete from products where id=" + id + "";
                    conn.Open();
                    cmd.ExecuteReader();
                    conn.Close();
                    Console.WriteLine("The product has been successfully deleted!");
                    AsAdmin();
                }
                if (decision == "Yes")
                {
                    DeleteProduct();
                } else
                {
                    AsAdmin();
                }
            }
            if(choice == "5")
            {
                Console.WriteLine("View Transaction");
                Console.WriteLine("================");
                SqlConnection conn = new SqlConnection(@"Data Source=.;Initial Catalog=marketDB;Integrated Security=True");
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from transactions";
                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Console.WriteLine("Transaction ID       : {0}", rdr["id"]);
                    Console.WriteLine("|No | Product Name | Quantity | Price |");
                    int grand_total = 0;
                    string payment_method = (string)rdr["method"];
                    void DisplayTransactionDetails(string transaction_id)
                    {
                        SqlConnection conn = new SqlConnection(@"Data Source=.;Initial Catalog=marketDB;Integrated Security=True");
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandText = "select * from transaction_details where transaction_id='"+ transaction_id + "'";
                        conn.Open();
                        SqlDataReader rdr1 = cmd.ExecuteReader();
                        int count = 1;
                        while(rdr1.Read())
                        {
                            void SearchThroughProduct(int product_id)
                            {
                                SqlConnection conn = new SqlConnection(@"Data Source=.;Initial Catalog=marketDB;Integrated Security=True");
                                SqlCommand cmd = new SqlCommand();
                                cmd.Connection = conn;
                                cmd.CommandText = "select * from products where id='" + product_id + "'";
                                conn.Open();
                                SqlDataReader rdr2 = cmd.ExecuteReader();
                                while(rdr2.Read())
                                {
                                    Console.WriteLine("|{0} | {1} | {2} | {3} |", count, rdr2["name"], rdr1["quantity"], rdr2["price"]);
                                    grand_total += ((int)rdr1["quantity"] * (int)rdr2["price"]);
                                }
                                conn.Close();
                            }
                            SearchThroughProduct((int)rdr1["product_id"]);
                            count++;
                        }
                        conn.Close();
                    }
                    DisplayTransactionDetails((string)rdr["id"]);
                    Console.WriteLine("Grand Total          : {0} by {1}", grand_total,payment_method);
                }
                conn.Close();
                AsAdmin();
            }
            if(choice == "6")
            {
                Main();
            }
            if(choice == "4")
            {
                SqlConnection conn = new SqlConnection(@"Data Source=.;Initial Catalog=marketDB;Integrated Security=True");
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from products";
                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    Console.WriteLine("View Product");
                    Console.WriteLine("============");
                    while (rdr.Read())
                    {
                        Console.WriteLine("Product ID       : {0}", rdr["id"]);
                        Console.WriteLine("Product Name     : {0}", rdr["name"]);
                        Console.WriteLine("Product Quantity : {0:#,0}", rdr["quantity"]);
                        Console.WriteLine("Product Price    : Rp {0:#,0}", rdr["price"]);
                        Console.WriteLine();
                    }
                    conn.Close();
                    AsAdmin();
                } else
                {
                    Console.WriteLine("No product available!");
                    AsAdmin();
                }
            }
        }
        // main function
        static void Main()
        {
            Console.WriteLine("Supermarket System");
            Console.WriteLine("==================");
            Console.WriteLine("1. Login as User");
            Console.WriteLine("2. Login as Admin");
            Console.WriteLine("3. Exit");
            Console.WriteLine("Choice:");
            string choice = Console.ReadLine();
            if(choice == "1")
            {
                AsUser();
            }
            if(choice == "2")
            {
                AsAdmin();
            }
            if(choice == "3")
            {
                Main();
            }
        }
    }
}
