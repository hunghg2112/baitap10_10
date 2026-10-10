using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BaiTapQuanLyTapTin
{
    // ==========================================
    // 1. LỚP ĐỐI TƯỢNG NHÂN VIÊN
    // ==========================================
    public class Employee
    {
        public string EmpId { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public DateTime JoinDate { get; set; }
        // Khóa để biết nhân viên này thuộc Phòng/Nhóm nào
        public string NodeKey { get; set; }
    }

    // ==========================================
    // 2. LỚP FORM CHÍNH (Giao diện & Logic)
    // ==========================================
    public class Form1 : Form
    {
        // Các Control giao diện
        private SplitContainer splitContainer;
        private TreeView tvDepartments;
        private ListView lsvEmployees;
        private ComboBox cboViewMode;
        private ImageList imgListSmall, imgListLarge;

        // Dữ liệu
        private List<Employee> allEmployees = new List<Employee>();

        public Form1()
        {
            TaoIconTuDong(); // Tạo icon bằng code
            TaoGiaoDien();   // Vẽ giao diện
            LoadData();      // Nạp dữ liệu giả lập
        }

        // --- HÀM TẠO ICON TỰ ĐỘNG (Tránh lỗi thiếu file ảnh) ---
        private void TaoIconTuDong()
        {
            imgListSmall = new ImageList { ImageSize = new Size(16, 16) };
            imgListLarge = new ImageList { ImageSize = new Size(32, 32) };

            // Hàm vẽ Icon nhanh
            Bitmap DrawIcon(Color bgColor, string shape, int size)
            {
                Bitmap bmp = new Bitmap(size, size);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Transparent);
                    using (Brush brush = new SolidBrush(bgColor))
                    {
                        if (shape == "Square") g.FillRectangle(brush, 2, 2, size - 4, size - 4);
                        else if (shape == "Circle") g.FillEllipse(brush, 2, 2, size - 4, size - 4);
                    }
                }
                return bmp;
            }

            // Index 0: Công ty (Hình vuông xanh dương)
            imgListSmall.Images.Add(DrawIcon(Color.DodgerBlue, "Square", 16));
            imgListLarge.Images.Add(DrawIcon(Color.DodgerBlue, "Square", 32));

            // Index 1: Phòng ban / Nhóm (Hình vuông vàng cam)
            imgListSmall.Images.Add(DrawIcon(Color.Orange, "Square", 16));
            imgListLarge.Images.Add(DrawIcon(Color.Orange, "Square", 32));

            // Index 2: Nhân viên (Hình tròn xanh lá)
            imgListSmall.Images.Add(DrawIcon(Color.LimeGreen, "Circle", 16));
            imgListLarge.Images.Add(DrawIcon(Color.LimeGreen, "Circle", 32));
        }

        // --- HÀM TẠO GIAO DIỆN ---
        private void TaoGiaoDien()
        {
            this.Text = "Trình Quản Lý Nhân Sự (TreeView & ListView)";
            this.Size = new Size(900, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9.5F);

            // 1. Tạo SplitContainer chia màn hình làm 2 nửa
            splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 250 // Độ rộng mảng bên trái
            };
            this.Controls.Add(splitContainer);

            // 2. Bên Trái: TreeView (Sơ đồ tổ chức)
            tvDepartments = new TreeView
            {
                Dock = DockStyle.Fill,
                ImageList = imgListSmall // Gắn ImageList cho TreeView
            };
            tvDepartments.AfterSelect += TvDepartments_AfterSelect;
            splitContainer.Panel1.Controls.Add(tvDepartments);

            // 3. Bên Phải: Có 1 Panel chứa ComboBox ở trên, và ListView ở dưới
            Panel pnlTopRight = new Panel { Dock = DockStyle.Top, Height = 40 };

            Label lblView = new Label { Text = "Chế độ xem:", Location = new Point(10, 12), AutoSize = true };
            pnlTopRight.Controls.Add(lblView);

            cboViewMode = new ComboBox
            {
                Location = new Point(100, 10),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            // Thêm các chế độ xem vào ComboBox
            cboViewMode.Items.AddRange(new object[] { "Details", "LargeIcon", "SmallIcon", "List", "Tile" });
            cboViewMode.SelectedIndexChanged += CboViewMode_SelectedIndexChanged;
            pnlTopRight.Controls.Add(cboViewMode);

            splitContainer.Panel2.Controls.Add(pnlTopRight);

            // 4. Bên Phải (Dưới): ListView
            lsvEmployees = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details, // Chế độ mặc định
                FullRowSelect = true,
                GridLines = true,
                SmallImageList = imgListSmall, // Gắn ImageList nhỏ
                LargeImageList = imgListLarge  // Gắn ImageList lớn
            };

            // YÊU CẦU 3: Cấu hình các cột ở chế độ Details
            lsvEmployees.Columns.Add("Mã NV", 80);
            lsvEmployees.Columns.Add("Họ Tên", 180);
            lsvEmployees.Columns.Add("Chức vụ", 120);
            lsvEmployees.Columns.Add("Ngày vào làm", 120);

            splitContainer.Panel2.Controls.Add(lsvEmployees);

            // Chọn mặc định chế độ Details trong ComboBox
            cboViewMode.SelectedIndex = 0;
        }

        // ==========================================
        // CÁC HÀM XỬ LÝ LOGIC & SỰ KIỆN
        // ==========================================

        private void LoadData()
        {
            // YÊU CẦU 1: Xây dựng cấu trúc cây (TreeView)
            // Node cha (Công ty)
            TreeNode rootNode = new TreeNode("Công ty TNHH ABC", 0, 0) { Tag = "ROOT" };

            // Phòng IT
            TreeNode nodeIT = new TreeNode("Phòng IT", 1, 1) { Tag = "ROOT_IT" };
            nodeIT.Nodes.Add(new TreeNode("Nhóm Dev", 1, 1) { Tag = "ROOT_IT_DEV" });
            nodeIT.Nodes.Add(new TreeNode("Nhóm QA", 1, 1) { Tag = "ROOT_IT_QA" });

            // Phòng Nhân sự
            TreeNode nodeHR = new TreeNode("Phòng Nhân sự", 1, 1) { Tag = "ROOT_HR" };

            // Phòng Kinh doanh
            TreeNode nodeSales = new TreeNode("Phòng Kinh doanh", 1, 1) { Tag = "ROOT_SALES" };

            // Thêm vào TreeView
            rootNode.Nodes.Add(nodeIT);
            rootNode.Nodes.Add(nodeHR);
            rootNode.Nodes.Add(nodeSales);
            tvDepartments.Nodes.Add(rootNode);

            tvDepartments.ExpandAll(); // Mở rộng toàn bộ cây

            // Nạp danh sách Nhân viên giả lập (Gắn Key tương ứng với Node Tag ở trên)
            allEmployees.Add(new Employee { EmpId = "NV01", FullName = "Nguyễn Văn Lập Trình", Position = "Dev Senior", JoinDate = new DateTime(2021, 5, 10), NodeKey = "ROOT_IT_DEV" });
            allEmployees.Add(new Employee { EmpId = "NV02", FullName = "Trần Thị Tester", Position = "QA Automation", JoinDate = new DateTime(2022, 8, 15), NodeKey = "ROOT_IT_QA" });
            allEmployees.Add(new Employee { EmpId = "NV03", FullName = "Lê Tuyển Dụng", Position = "HR Manager", JoinDate = new DateTime(2020, 1, 20), NodeKey = "ROOT_HR" });
            allEmployees.Add(new Employee { EmpId = "NV04", FullName = "Phạm Bán Hàng", Position = "Sales Executive", JoinDate = new DateTime(2023, 3, 5), NodeKey = "ROOT_SALES" });
            allEmployees.Add(new Employee { EmpId = "NV05", FullName = "Hoàng Chốt Đơn", Position = "Sales Leader", JoinDate = new DateTime(2019, 11, 12), NodeKey = "ROOT_SALES" });
        }

        // YÊU CẦU 2: Click vào TreeView -> Nạp nhân viên lên ListView
        private void TvDepartments_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null || e.Node.Tag == null) return;

            string selectedTag = e.Node.Tag.ToString();

            // Xóa danh sách cũ trên ListView
            lsvEmployees.Items.Clear();

            // Lọc danh sách nhân viên: 
            // Nếu NodeKey của nhân viên bắt đầu bằng selectedTag thì lấy 
            // (Mục đích: Nếu click vào "Phòng IT", nó sẽ lấy luôn cả nhân viên nhóm Dev và QA)
            var filteredEmployees = allEmployees.Where(emp => emp.NodeKey.StartsWith(selectedTag)).ToList();

            foreach (var emp in filteredEmployees)
            {
                // Tạo một dòng mới (ListViewItem)
                ListViewItem item = new ListViewItem(emp.EmpId);
                item.ImageIndex = 2; // Dùng Icon nhân viên (Hình tròn xanh)

                // Thêm các cột phụ (SubItems) cho chế độ Details
                item.SubItems.Add(emp.FullName);
                item.SubItems.Add(emp.Position);
                item.SubItems.Add(emp.JoinDate.ToString("dd/MM/yyyy"));

                // Đưa lên giao diện
                lsvEmployees.Items.Add(item);
            }
        }

        // Sự kiện: Đổi chế độ xem (View) của ListView thông qua ComboBox
        private void CboViewMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            string mode = cboViewMode.SelectedItem.ToString();
            switch (mode)
            {
                case "Details": lsvEmployees.View = View.Details; break;
                case "LargeIcon": lsvEmployees.View = View.LargeIcon; break;
                case "SmallIcon": lsvEmployees.View = View.SmallIcon; break;
                case "List": lsvEmployees.View = View.List; break;
                case "Tile": lsvEmployees.View = View.Tile; break;
            }
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
            Application.Run(new Form1());
        }
    }
}