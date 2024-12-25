using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;
using De02.Model;

namespace De02
{
    public partial class frmSanpham : Form
    {
        private Model1 db;
        private bool isAdding;

        public frmSanpham()
        {
            InitializeComponent();
            LoadData();
            LoadLoaiSP();
            SetControlState(false);
        }

        private void LoadData()
        {
            db = new Model1();

            try
            {
                var sanphams = db.Sanphams.Include(s => s.LoaiSP).ToList();

                dgvSanpham.Rows.Clear();
                foreach (var sp in sanphams)
                {
                    dgvSanpham.Rows.Add(sp.MaSP, sp.TenSP, sp.Ngaynhap, sp.LoaiSP.TenLoai);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void LoadLoaiSP()
        {
            db = new Model1();

            try
            {
                var loaiSps = db.LoaiSPs.ToList();
                cboLoaiSP.DataSource = loaiSps;
                cboLoaiSP.DisplayMember = "TenLoai";
                cboLoaiSP.ValueMember = "MaLoai";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void dgvSanpham_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvSanpham.Rows[e.RowIndex];
                txtMaSP.Text = row.Cells["MaSP"].Value.ToString();
                txtTenSP.Text = row.Cells["TenSP"].Value.ToString();

                if (DateTime.TryParse(row.Cells["Ngaynhap"].Value.ToString(), out DateTime ngaynhap))
                {
                    dtNgaynhap.Value = ngaynhap;
                }
                else
                {
                    MessageBox.Show("Giá trị ngày nhập không hợp lệ.");
                }

                cboLoaiSP.Text = row.Cells["LoaiSP"].Value.ToString();
                SetControlState(false);
            }
        }

        private void SetControlState(bool isEditing)
        {
            txtMaSP.ReadOnly = !isEditing;
            txtTenSP.ReadOnly = !isEditing;
            dtNgaynhap.Enabled = isEditing;
            cboLoaiSP.Enabled = isEditing;

            btThem.Enabled = !isEditing;
            btSua.Enabled = !isEditing && !string.IsNullOrEmpty(txtMaSP.Text);
            btXoa.Enabled = !isEditing && !string.IsNullOrEmpty(txtMaSP.Text);
            btLuu.Enabled = isEditing;
            btKLuu.Enabled = isEditing;
        }

        private void btThem_Click(object sender, EventArgs e)
        {
            isAdding = true;
            SetControlState(true);
            ClearControls();
            txtMaSP.Focus();
        }

        private void btSua_Click(object sender, EventArgs e)
        {
            isAdding = false;
            SetControlState(true);
            txtMaSP.ReadOnly = true; // Không cho phép sửa Mã Sản Phẩm
        }

        private void btXoa_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm này?", "Xóa sản phẩm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    var maSP = txtMaSP.Text;
                    var sanpham = db.Sanphams.SingleOrDefault(sp => sp.MaSP == maSP);
                    if (sanpham != null)
                    {
                        db.Sanphams.Remove(sanpham);
                        db.SaveChanges();
                        LoadData();
                        ClearControls();
                        SetControlState(false);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private void btLuu_Click(object sender, EventArgs e)
        {
            if (isAdding)
            {
                var newSanpham = new Sanpham
                {
                    MaSP = txtMaSP.Text,
                    TenSP = txtTenSP.Text,
                    Ngaynhap = dtNgaynhap.Value,
                    MaLoai = cboLoaiSP.SelectedValue.ToString()
                };

                try
                {
                    db.Sanphams.Add(newSanpham);
                    db.SaveChanges();
                    LoadData();
                    ClearControls();
                    SetControlState(false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
            else
            {
                var maSP = txtMaSP.Text;
                var sanpham = db.Sanphams.SingleOrDefault(sp => sp.MaSP == maSP);

                if (sanpham != null)
                {
                    sanpham.TenSP = txtTenSP.Text;
                    sanpham.Ngaynhap = dtNgaynhap.Value;
                    sanpham.MaLoai = cboLoaiSP.SelectedValue.ToString();

                    try
                    {
                        db.SaveChanges();
                        LoadData();
                        ClearControls();
                        SetControlState(false);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi: " + ex.Message);
                    }
                }
            }
        }

        private void btKLuu_Click(object sender, EventArgs e)
        {
            ClearControls();
            SetControlState(false);
        }

        private void ClearControls()
        {
            txtMaSP.Clear();
            txtTenSP.Clear();
            dtNgaynhap.Value = DateTime.Now;
            cboLoaiSP.SelectedIndex = -1;
        }

        private void btThoat_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn đóng form này?", "Đóng form", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void btTim_Click(object sender, EventArgs e)
        {
            string timKiem = txtTim.Text.ToLower();
            var ketQua = db.Sanphams.Include(s => s.LoaiSP).Where(sp => sp.TenSP.ToLower().Contains(timKiem)).ToList();

            dgvSanpham.Rows.Clear();
            foreach (var sp in ketQua)
            {
                dgvSanpham.Rows.Add(sp.MaSP, sp.TenSP, sp.Ngaynhap, sp.LoaiSP.TenLoai);
            }

            if (ketQua.Count == 0)
            {
                MessageBox.Show("Không tìm thấy sản phẩm nào.");
            }
        }
    }
}
