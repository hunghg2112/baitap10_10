using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BaiTapQuanLySanPham
{
    // ==========================================
    // 1. LỚP ĐỐI TƯỢNG SẢN PHẨM
    // ==========================================
    public class Product
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }
    }

    // ==========================================
    // 2. LỚP FORM CHÍNH (Giao diện & Logic)
    // ==========================================
    public class Form1 : Form
    {
        // Khai báo các công cụ giao diện
        private TextBox txtProductId, txtProductName, txtCategory, txtSearch;
        private NumericUpDown numUnitPrice, numQuantity;
        private Button btnAdd, btnEdit, btnDelete, btnSearch;
        private DataGridView dgvProducts;

        // Biến lưu trữ dữ liệu
        private List<Product> productList = new List<Product>();
        private BindingSource bindingSource = new BindingSource();

        public Form1()
        {
            // Tự động tạo giao diện bằng code
            TaoGiaoDien();

            // Kết nối dữ liệu vào bảng
            bindingSource.DataSource = productList;
            dgvProducts.DataSource = bindingSource;

            // Thêm 2 sản phẩm mẫu ban đầu
            productList.Add(new Product { ProductId = "SP01", ProductName = "Laptop ThinkPad T460", UnitPrice = 12000000, Quantity = 5, Category = "Laptop" });
            productList.Add(new Product { ProductId = "SP02", ProductName = "Điện thoại Tecno", UnitPrice = 4500000, Quantity = 10, Category = "Smartphone" });

            RefreshGrid();
        }

        private void TaoGiaoDien()
        {
            // Thiết lập Form
            this.Text = "Quản Lý Sản Phẩm (Tất cả trong 1 file)";
            this.Size = new Size(820, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9F);

            // --- TẠO CÁC Ô NHẬP LIỆU ---
            this.Controls.Add(new Label { Text = "Mã SP:", Location = new Point(20, 20), AutoSize = true });
            txtProductId = new TextBox { Location = new Point(80, 18), Width = 150 };
            this.Controls.Add(txtProductId);

            this.Controls.Add(new Label { Text = "Tên SP:", Location = new Point(20, 60), AutoSize = true });
            txtProductName = new TextBox { Location = new Point(80, 58), Width = 150 };
            this.Controls.Add(txtProductName);

            this.Controls.Add(new Label { Text = "Đơn giá:", Location = new Point(260, 20), AutoSize = true });
            numUnitPrice = new NumericUpDown { Location = new Point(330, 18), Width = 150, Maximum = 1000000000 };
            this.Controls.Add(numUnitPrice);

            this.Controls.Add(new Label { Text = "Số lượng:", Location = new Point(260, 60), AutoSize = true });
            numQuantity = new NumericUpDown { Location = new Point(330, 58), Width = 150, Maximum = 10000 };
            this.Controls.Add(numQuantity);

            this.Controls.Add(new Label { Text = "Danh mục:", Location = new Point(520, 20), AutoSize = true });
            txtCategory = new TextBox { Location = new Point(590, 18), Width = 150 };
            this.Controls.Add(txtCategory);

            // --- TẠO CÁC NÚT BẤM ---
            btnAdd = new Button { Text = "Thêm", Location = new Point(20, 100), Width = 80 };
            btnAdd.Click += BtnAdd_Click;
            this.Controls.Add(btnAdd);

            btnEdit = new Button { Text = "Sửa", Location = new Point(110, 100), Width = 80 };
            btnEdit.Click += BtnEdit_Click;
            this.Controls.Add(btnEdit);

            btnDelete = new Button { Text = "Xóa", Location = new Point(200, 100), Width = 80 };
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);

            // --- TẠO Ô TÌM KIẾM ---
            txtSearch = new TextBox { Location = new Point(480, 102), Width = 180 };
            this.Controls.Add(txtSearch);

            btnSearch = new Button { Text = "Tìm kiếm", Location = new Point(670, 100), Width = 80 };
            btnSearch.Click += BtnSearch_Click;
            this.Controls.Add(btnSearch);

            // --- TẠO BẢNG DATAGRIDVIEW ---
            dgvProducts = new DataGridView
            {
                Location = new Point(20, 150),
                Size = new Size(760, 290),
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvProducts.CellClick += DgvProducts_CellClick;
            this.Controls.Add(dgvProducts);
        }

        // ==========================================
        // CÁC HÀM XỬ LÝ SỰ KIỆN (LOGIC)
        // ==========================================
        private void RefreshGrid()
        {
            bindingSource.ResetBindings(false);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProductId.Text) || string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("Vui lòng nhập đủ Mã và Tên sản phẩm!", "Lỗi");
                return;
            }

            if (productList.Any(p => p.ProductId == txtProductId.Text))
            {
                MessageBox.Show("Mã sản phẩm đã tồn tại!", "Lỗi");
                return;
            }

            productList.Add(new Product
            {
                ProductId = txtProductId.Text,
                ProductName = txtProductName.Text,
                UnitPrice = numUnitPrice.Value,
                Quantity = (int)numQuantity.Value,
                Category = txtCategory.Text
            });

            RefreshGrid();
            ClearInput();
        }

        private void DgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            Product selected = (Product)bindingSource.Current;
            if (selected != null)
            {
                txtProductId.Text = selected.ProductId;
                txtProductName.Text = selected.ProductName;
                numUnitPrice.Value = selected.UnitPrice;
                numQuantity.Value = selected.Quantity;
                txtCategory.Text = selected.Category;

                txtProductId.Enabled = false; // Khóa mã SP không cho sửa
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (bindingSource.Current != null)
            {
                Product selected = (Product)bindingSource.Current;
                selected.ProductName = txtProductName.Text;
                selected.UnitPrice = numUnitPrice.Value;
                selected.Quantity = (int)numQuantity.Value;
                selected.Category = txtCategory.Text;

                RefreshGrid();
                MessageBox.Show("Cập nhật thành công!");
                ClearInput();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (bindingSource.Current != null)
            {
                Product selected = (Product)bindingSource.Current;
                if (MessageBox.Show($"Bạn có chắc muốn xóa '{selected.ProductName}'?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    productList.Remove(selected);
                    RefreshGrid();
                    ClearInput();
                }
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.ToLower().Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                bindingSource.DataSource = productList;
            }
            else
            {
                bindingSource.DataSource = productList.Where(p => p.ProductName.ToLower().Contains(keyword)).ToList();
            }
            RefreshGrid();
        }

        private void ClearInput()
        {
            txtProductId.Clear();
            txtProductName.Clear();
            txtCategory.Clear();
            numUnitPrice.Value = 0;
            numQuantity.Value = 0;
            txtProductId.Enabled = true;
            txtProductId.Focus();
        }
    }

    // ==========================================
    // 3. ĐIỂM CHẠY CỦA CHƯƠNG TRÌNH 
    // (Bắt buộc phải có trong C# Winforms)
    // ==========================================
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1()); // Khởi chạy Form1
        }
    }
}