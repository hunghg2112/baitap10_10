using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BaiTapTinhTienDichVu
{
    // ==========================================
    // 1. LỚP ĐỐI TƯỢNG LƯU TRỮ DỊCH VỤ
    // ==========================================
    public class Service
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }

        // Hiển thị đẹp trên ListBox (Ví dụ: Khám nội - 150,000 VNĐ)
        public override string ToString()
        {
            return $"{Name} - {Price:N0} VNĐ";
        }
    }

    // ==========================================
    // 2. LỚP FORM CHÍNH (Giao diện & Logic)
    // ==========================================
    public class Form1 : Form
    {
        // Khai báo các công cụ giao diện
        private ComboBox cboCategory;
        private ListBox lstAvailableServices, lstSelectedServices;
        private Button btnSelect, btnRemove, btnClearAll;
        private TextBox txtTotal, txtFinalAmount;
        private NumericUpDown numDiscount;

        // Danh sách chứa TẤT CẢ các dịch vụ trong hệ thống
        private List<Service> allServices = new List<Service>();

        public Form1()
        {
            TaoGiaoDien(); // Vẽ giao diện
            LoadData();    // Nạp dữ liệu giả lập
        }

        private void TaoGiaoDien()
        {
            // Thiết lập Form
            this.Text = "Bảng Tính Tiền Dịch Vụ (Tất cả trong 1 file)";
            this.Size = new Size(700, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9F);

            // --- Chọn danh mục ---
            this.Controls.Add(new Label { Text = "Chọn loại dịch vụ:", Location = new Point(20, 20), AutoSize = true });
            cboCategory = new ComboBox { Location = new Point(140, 18), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cboCategory.SelectedIndexChanged += CboCategory_SelectedIndexChanged;
            this.Controls.Add(cboCategory);

            // --- Cột trái: Dịch vụ có sẵn ---
            this.Controls.Add(new Label { Text = "Danh sách dịch vụ:", Location = new Point(20, 60), AutoSize = true });
            lstAvailableServices = new ListBox { Location = new Point(20, 80), Size = new Size(250, 200) };
            lstAvailableServices.DoubleClick += BtnSelect_Click; // Hỗ trợ nhấp đúp (Double Click)
            this.Controls.Add(lstAvailableServices);

            // --- Cột giữa: Các nút bấm thao tác ---
            btnSelect = new Button { Text = ">", Location = new Point(290, 100), Size = new Size(50, 35) };
            btnSelect.Click += BtnSelect_Click;
            this.Controls.Add(btnSelect);

            btnRemove = new Button { Text = "<", Location = new Point(290, 150), Size = new Size(50, 35) };
            btnRemove.Click += BtnRemove_Click;
            this.Controls.Add(btnRemove);

            btnClearAll = new Button { Text = "<<", Location = new Point(290, 200), Size = new Size(50, 35) };
            btnClearAll.Click += BtnClearAll_Click;
            this.Controls.Add(btnClearAll);

            // --- Cột phải: Dịch vụ đã chọn ---
            this.Controls.Add(new Label { Text = "Dịch vụ đã chọn:", Location = new Point(360, 60), AutoSize = true });
            lstSelectedServices = new ListBox { Location = new Point(360, 80), Size = new Size(300, 200) };
            this.Controls.Add(lstSelectedServices);

            // --- Khu vực tính toán thanh toán (Góc dưới bên phải) ---
            int bottomY = 295;
            this.Controls.Add(new Label { Text = "Tổng tiền chưa giảm:", Location = new Point(360, bottomY), AutoSize = true });
            txtTotal = new TextBox { Location = new Point(500, bottomY - 2), Width = 160, ReadOnly = true, TextAlign = HorizontalAlignment.Right };
            this.Controls.Add(txtTotal);

            this.Controls.Add(new Label { Text = "Tỷ lệ chiết khấu (%):", Location = new Point(360, bottomY + 30), AutoSize = true });
            numDiscount = new NumericUpDown { Location = new Point(500, bottomY + 28), Width = 160, Maximum = 100, TextAlign = HorizontalAlignment.Right };
            numDiscount.ValueChanged += NumDiscount_ValueChanged;
            this.Controls.Add(numDiscount);

            this.Controls.Add(new Label { Text = "Thành tiền:", Location = new Point(360, bottomY + 60), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) });
            txtFinalAmount = new TextBox { Location = new Point(500, bottomY + 58), Width = 160, ReadOnly = true, TextAlign = HorizontalAlignment.Right, Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.Red };
            this.Controls.Add(txtFinalAmount);
        }

        // ==========================================
        // CÁC HÀM XỬ LÝ SỰ KIỆN (LOGIC)
        // ==========================================

        private void LoadData()
        {
            // Thêm dữ liệu mẫu vào danh sách gốc
            allServices.Add(new Service { Name = "Khám nội chung", Category = "Khám bệnh", Price = 150000 });
            allServices.Add(new Service { Name = "Khám chuyên khoa", Category = "Khám bệnh", Price = 300000 });
            allServices.Add(new Service { Name = "Xét nghiệm máu", Category = "Xét nghiệm", Price = 250000 });
            allServices.Add(new Service { Name = "Xét nghiệm nước tiểu", Category = "Xét nghiệm", Price = 100000 });
            allServices.Add(new Service { Name = "Chụp X-Quang Phổi", Category = "Chụp X-Quang", Price = 200000 });
            allServices.Add(new Service { Name = "Chụp X-Quang Xương", Category = "Chụp X-Quang", Price = 250000 });
            allServices.Add(new Service { Name = "Vắc-xin Cúm", Category = "Vắc-xin", Price = 350000 });
            allServices.Add(new Service { Name = "Vắc-xin Viêm gan B", Category = "Vắc-xin", Price = 450000 });

            // Thêm các loại danh mục vào ComboBox
            cboCategory.Items.AddRange(new string[] { "Khám bệnh", "Xét nghiệm", "Chụp X-Quang", "Vắc-xin" });

            // Tự động chọn mục đầu tiên
            if (cboCategory.Items.Count > 0)
                cboCategory.SelectedIndex = 0;
        }

        // YÊU CẦU 1: Lọc dữ liệu khi đổi ComboBox
        private void CboCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCategory.SelectedItem == null) return;

            string selectedCat = cboCategory.SelectedItem.ToString();
            lstAvailableServices.Items.Clear(); // Xóa sạch bảng bên trái

            foreach (var s in allServices)
            {
                if (s.Category == selectedCat)
                    lstAvailableServices.Items.Add(s);
            }
        }

        // YÊU CẦU 2: Nút Select (>) hoặc Double click
        private void BtnSelect_Click(object sender, EventArgs e)
        {
            if (lstAvailableServices.SelectedItem != null)
            {
                // Chuyển đối tượng sang Listbox bên phải
                lstSelectedServices.Items.Add(lstAvailableServices.SelectedItem);
                CalculateTotal(); // Tính lại tiền
            }
            else if (sender == btnSelect)
            {
                MessageBox.Show("Vui lòng chọn một dịch vụ ở danh sách bên trái!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Nút Remove (<)
        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (lstSelectedServices.SelectedItem != null)
            {
                lstSelectedServices.Items.Remove(lstSelectedServices.SelectedItem);
                CalculateTotal();
            }
        }

        // Nút Clear All (<<)
        private void BtnClearAll_Click(object sender, EventArgs e)
        {
            lstSelectedServices.Items.Clear();
            CalculateTotal();
        }

        // YÊU CẦU 3: Hàm tự động tính toán tổng tiền
        private void CalculateTotal()
        {
            decimal totalBeforeDiscount = 0;

            // Lặp qua danh sách đã chọn để cộng tiền
            foreach (Service item in lstSelectedServices.Items)
            {
                totalBeforeDiscount += item.Price;
            }

            txtTotal.Text = totalBeforeDiscount.ToString("N0") + " VNĐ";

            // Tính tiền sau chiết khấu
            decimal discountRate = numDiscount.Value;
            decimal discountAmount = totalBeforeDiscount * (discountRate / 100);
            decimal finalAmount = totalBeforeDiscount - discountAmount;

            txtFinalAmount.Text = finalAmount.ToString("N0") + " VNĐ";
        }

        // Tự động tính tiền mỗi khi tỷ lệ chiết khấu bị thay đổi
        private void NumDiscount_ValueChanged(object sender, EventArgs e)
        {
            CalculateTotal();
        }
    }

    // ==========================================
    // 3. ĐIỂM CHẠY CỦA CHƯƠNG TRÌNH 
    // ==========================================
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1()); // Khởi chạy Form
        }
    }
}