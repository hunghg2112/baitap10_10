using System;
using System.Drawing;
using System.Windows.Forms;

public class RegisterForm : Form
{
    // Khai báo các GroupBox
    private GroupBox grpPersonalInfo;
    private GroupBox grpAdditionalInfo;

    // Controls Nhóm Thông tin cá nhân
    private TextBox txtUsername, txtPassword, txtConfirmPassword;

    // Controls Nhóm Thông tin bổ sung
    private DateTimePicker dtpDob;
    private RadioButton rdoMale, rdoFemale;
    private CheckBox chkTerms;

    // Controls Chức năng & Bắt lỗi
    private Button btnRegister, btnReset;
    private ErrorProvider epCheck;

    public RegisterForm()
    {
        SetupUI();
    }

    private void SetupUI()
    {
        this.Text = "Form Đăng ký Tài khoản";
        this.Size = new Size(450, 520);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Font = new Font("Arial", 10);

        epCheck = new ErrorProvider(this);

        // ================= NHÓM 1: THÔNG TIN CÁ NHÂN =================
        grpPersonalInfo = new GroupBox { Text = "Thông tin cá nhân", Location = new Point(20, 20), Size = new Size(390, 160) };
        this.Controls.Add(grpPersonalInfo);

        int startY = 30;
        int gapY = 40;

        grpPersonalInfo.Controls.Add(new Label { Text = "Tên đăng nhập:", Location = new Point(20, startY), AutoSize = true });
        txtUsername = new TextBox { Location = new Point(150, startY - 3), Width = 200 };
        grpPersonalInfo.Controls.Add(txtUsername);

        startY += gapY;
        grpPersonalInfo.Controls.Add(new Label { Text = "Mật khẩu:", Location = new Point(20, startY), AutoSize = true });
        txtPassword = new TextBox { Location = new Point(150, startY - 3), Width = 200, UseSystemPasswordChar = true }; // Ẩn ký tự mật khẩu
        grpPersonalInfo.Controls.Add(txtPassword);

        startY += gapY;
        grpPersonalInfo.Controls.Add(new Label { Text = "Xác nhận MK:", Location = new Point(20, startY), AutoSize = true });
        txtConfirmPassword = new TextBox { Location = new Point(150, startY - 3), Width = 200, UseSystemPasswordChar = true };
        grpPersonalInfo.Controls.Add(txtConfirmPassword);


        // ================= NHÓM 2: THÔNG TIN BỔ SUNG =================
        grpAdditionalInfo = new GroupBox { Text = "Thông tin bổ sung", Location = new Point(20, 200), Size = new Size(390, 140) };
        this.Controls.Add(grpAdditionalInfo);

        startY = 30;
        grpAdditionalInfo.Controls.Add(new Label { Text = "Ngày sinh:", Location = new Point(20, startY), AutoSize = true });
        dtpDob = new DateTimePicker { Location = new Point(150, startY - 3), Width = 200, Format = DateTimePickerFormat.Short };
        grpAdditionalInfo.Controls.Add(dtpDob);

        startY += gapY;
        grpAdditionalInfo.Controls.Add(new Label { Text = "Giới tính:", Location = new Point(20, startY), AutoSize = true });
        rdoMale = new RadioButton { Text = "Nam", Location = new Point(150, startY), AutoSize = true, Checked = true };
        rdoFemale = new RadioButton { Text = "Nữ", Location = new Point(220, startY), AutoSize = true };
        grpAdditionalInfo.Controls.Add(rdoMale);
        grpAdditionalInfo.Controls.Add(rdoFemale);

        startY += gapY;
        chkTerms = new CheckBox { Text = "Tôi đồng ý với Điều khoản dịch vụ", Location = new Point(150, startY), AutoSize = true };
        grpAdditionalInfo.Controls.Add(chkTerms);


        // ================= NHÓM 3: NÚT CHỨC NĂNG =================
        btnRegister = new Button { Text = "Đăng Ký", Location = new Point(80, 380), Size = new Size(120, 40), BackColor = Color.LightGreen };
        btnRegister.Click += BtnRegister_Click;
        this.Controls.Add(btnRegister);

        btnReset = new Button { Text = "Làm Mới", Location = new Point(230, 380), Size = new Size(120, 40) };
        btnReset.Click += BtnReset_Click;
        this.Controls.Add(btnReset);
    }

    // Sự kiện Đăng ký và bắt lỗi
    private void BtnRegister_Click(object sender, EventArgs e)
    {
        // Xóa toàn bộ icon báo lỗi cũ trước khi kiểm tra lại
        epCheck.Clear();
        bool isValid = true;

        // 1. Kiểm tra Tên đăng nhập
        if (string.IsNullOrWhiteSpace(txtUsername.Text))
        {
            epCheck.SetError(txtUsername, "Tên đăng nhập không được để trống!");
            isValid = false;
        }

        // 2. Kiểm tra Mật khẩu
        if (string.IsNullOrWhiteSpace(txtPassword.Text))
        {
            epCheck.SetError(txtPassword, "Mật khẩu không được để trống!");
            isValid = false;
        }

        // 3. Kiểm tra Xác nhận mật khẩu
        if (txtPassword.Text != txtConfirmPassword.Text)
        {
            epCheck.SetError(txtConfirmPassword, "Mật khẩu nhập lại không khớp!");
            isValid = false;
        }

        // 4. Kiểm tra Độ tuổi (>= 18 tuổi)
        int age = DateTime.Now.Year - dtpDob.Value.Year;
        // Hiệu chỉnh nếu chưa tới sinh nhật trong năm nay
        if (dtpDob.Value.Date > DateTime.Now.AddYears(-age))
        {
            age--;
        }

        if (age < 18)
        {
            epCheck.SetError(dtpDob, "Bạn phải đủ 18 tuổi trở lên để đăng ký!");
            isValid = false;
        }

        // 5. Kiểm tra Điều khoản dịch vụ
        if (!chkTerms.Checked)
        {
            epCheck.SetError(chkTerms, "Bạn phải đồng ý với Điều khoản dịch vụ!");
            isValid = false;
        }

        // Nếu tất cả hợp lệ, hiển thị thông báo thành công
        if (isValid)
        {
            MessageBox.Show($"Chúc mừng {txtUsername.Text} đã đăng ký tài khoản thành công!", "Đăng ký thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    // Sự kiện Làm mới form
    private void BtnReset_Click(object sender, EventArgs e)
    {
        txtUsername.Clear();
        txtPassword.Clear();
        txtConfirmPassword.Clear();
        dtpDob.Value = DateTime.Now;
        rdoMale.Checked = true;
        chkTerms.Checked = false;
        epCheck.Clear(); // Xóa sạch thông báo lỗi
        txtUsername.Focus();
    }

    // Hàm Main cho Mono/Linux
    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new RegisterForm());
    }
}