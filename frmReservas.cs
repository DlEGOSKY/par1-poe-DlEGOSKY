//
// PARCIAL I - POE
// Alumno: Diego Molina
// Código: U20240196
// Autoevaluación: 8.5/10
//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace u20240196
{
    public partial class frmReservas : Form
    {
        // Controles del formulario
        Label lblNombre, lblDUI, lblCategoria, lblPelicula, lblCantidad;
        TextBox txtNombre;
        MaskedTextBox txtDUI;
        ComboBox cmbCategoria, cmbPelicula;
        NumericUpDown nudCantidad;
        Button btnAgregar;
        DataGridView dgvReservas;

        // Diccionario para relacionar categorías con películas
        Dictionary<string, List<string>> catalogo = new Dictionary<string, List<string>>
        {
            { "Acción",   new List<string> { "Misión Imposible", "John Wick", "Mad Max" } },
            { "Comedia",  new List<string> { "Superbad", "La Gran Apuesta", "21 Jump Street" } },
            { "Animada",  new List<string> { "Toy Story", "Coco", "Spider-Verse" } },
            { "Terror",   new List<string> { "El Conjuro", "It", "Hereditary" } },
            { "Drama",    new List<string> { "La La Land", "Parasite", "Whiplash" } }
        };

        public frmReservas()
        {
            InitializeComponentCustom();
            HookEventos();
        }

        void InitializeComponentCustom()
        {
            // Propiedades 
            Name = "frmReservas";
            Text = "Crear Reservas";
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10f);
            ClientSize = new Size(720, 480);

            // Controles con los nombres que pide la guía
            lblNombre = new Label { Name = "lblNombre", Text = "Nombre:", AutoSize = true };
            txtNombre = new TextBox { Name = "txtNombre", Width = 250 };

            lblDUI = new Label { Name = "lblDUI", Text = "DUI:", AutoSize = true };
            txtDUI = new MaskedTextBox { Name = "txtDUI", Mask = "00000000-0", Width = 250 };

            lblCategoria = new Label { Name = "lblCategoria", Text = "Categoria", AutoSize = true };
            cmbCategoria = new ComboBox { Name = "cmbCategoria", DropDownStyle = ComboBoxStyle.DropDownList, Width = 250 };

            lblPelicula = new Label { Name = "lbPelicula", Text = "Pelicula", AutoSize = true };
            cmbPelicula = new ComboBox { Name = "cmbPelicula", DropDownStyle = ComboBoxStyle.DropDownList, Width = 250 };

            lblCantidad = new Label { Text = "Cantidad:", AutoSize = true };
            nudCantidad = new NumericUpDown { Width = 100, Minimum = 1, Maximum = 20, Value = 1 };

            btnAgregar = new Button { Name = "btnAgregar", Text = "Agregar", AutoSize = true, Enabled = false };

            dgvReservas = new DataGridView
            {
                Name = "dgvReservas",
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Layout para organizar los campos
            var grid = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 2,
                Padding = new Padding(16),
                AutoSize = true
            };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            int r = 0;
            grid.Controls.Add(lblNombre, 0, r);
            grid.Controls.Add(txtNombre, 1, r++);
            grid.Controls.Add(lblDUI, 0, r);
            grid.Controls.Add(txtDUI, 1, r++);
            grid.Controls.Add(lblCategoria, 0, r);
            grid.Controls.Add(cmbCategoria, 1, r++);
            grid.Controls.Add(lblPelicula, 0, r);
            grid.Controls.Add(cmbPelicula, 1, r++);
            grid.Controls.Add(lblCantidad, 0, r);
            grid.Controls.Add(nudCantidad, 1, r++);

            var bottom = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(16),
                AutoSize = true
            };
            bottom.Controls.Add(btnAgregar);

            var panelMain = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3 };
            panelMain.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panelMain.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            panelMain.Controls.Add(grid, 0, 0);
            panelMain.Controls.Add(bottom, 0, 1);
            panelMain.Controls.Add(dgvReservas, 0, 2);

            Controls.Add(panelMain);

            // Columnas del DataGrid
            dgvReservas.Columns.Add("colNombre", "Nombre");
            dgvReservas.Columns.Add("colDUI", "DUI");
            dgvReservas.Columns.Add("colCategoria", "Categoria");
            dgvReservas.Columns.Add("colPelicula", "Pelicula");
            dgvReservas.Columns.Add("colCantidad", "Cantidad");

            Load += frmReservas_Load;
        }

        void HookEventos()
        {
            txtNombre.KeyPress += SoloLetras; // validación
            cmbCategoria.SelectedIndexChanged += cmbCategoria_SelectedIndexChanged;
            btnAgregar.Click += btnAgregar_Click;

            // chequea cada vez que cambia algo
            txtNombre.TextChanged += CualquierCambio_ActualizarEstado;
            txtDUI.TextChanged += CualquierCambio_ActualizarEstado;
            cmbCategoria.SelectedIndexChanged += CualquierCambio_ActualizarEstado;
            cmbPelicula.SelectedIndexChanged += CualquierCambio_ActualizarEstado;
            nudCantidad.ValueChanged += (s, e) => CualquierCambio_ActualizarEstado(s, e);
        }

        void frmReservas_Load(object sender, EventArgs e)
        {
            // Cargar categorías en el combo
            cmbCategoria.Items.Clear();
            foreach (var cat in catalogo.Keys)
                cmbCategoria.Items.Add(cat);

            // limpiar campos al inicio
            cmbCategoria.SelectedIndex = -1;
            cmbPelicula.Items.Clear();
            cmbPelicula.SelectedIndex = -1;
            txtNombre.Clear();
            txtDUI.Clear();
            nudCantidad.Value = 1;
            btnAgregar.Enabled = false;
        }

        void cmbCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbPelicula.Items.Clear();
            cmbPelicula.SelectedIndex = -1;

            if (cmbCategoria.SelectedIndex >= 0)
            {
                var categoria = cmbCategoria.SelectedItem.ToString();
                if (catalogo.ContainsKey(categoria))
                {
                    foreach (var p in catalogo[categoria])
                        cmbPelicula.Items.Add(p);
                }
            }
        }

        void btnAgregar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string dui = txtDUI.Text;
            string categoria = cmbCategoria.SelectedIndex >= 0 ? cmbCategoria.SelectedItem.ToString() : "";
            string pelicula = cmbPelicula.SelectedIndex >= 0 ? cmbPelicula.SelectedItem.ToString() : "";
            int cantidad = (int)nudCantidad.Value;

            if (string.IsNullOrWhiteSpace(nombre) ||
                !txtDUI.MaskFull ||
                string.IsNullOrWhiteSpace(categoria) ||
                string.IsNullOrWhiteSpace(pelicula) ||
                cantidad <= 0)
            {
                MessageBox.Show("Completa todos los campos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Agregar la reserva al DataGri
            dgvReservas.Rows.Add(nombre, dui, categoria, pelicula, cantidad);

            // limpiar para otra reserva
            txtNombre.Clear();
            txtDUI.Clear();
            cmbCategoria.SelectedIndex = -1;
            cmbPelicula.Items.Clear();
            cmbPelicula.SelectedIndex = -1;
            nudCantidad.Value = 1;

            CualquierCambio_ActualizarEstado(null, EventArgs.Empty);
        }

        void SoloLetras(object sender, KeyPressEventArgs e)
        {
            // permitir solo letras y espacios
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
                e.Handled = true;
        }

        void CualquierCambio_ActualizarEstado(object sender, EventArgs e)
        {
            bool todoLleno =
                !string.IsNullOrWhiteSpace(txtNombre.Text) &&
                txtDUI.MaskFull &&
                cmbCategoria.SelectedIndex >= 0 &&
                cmbPelicula.SelectedIndex >= 0 &&
                nudCantidad.Value > 0;

            btnAgregar.Enabled = todoLleno;
        }
    }
}
