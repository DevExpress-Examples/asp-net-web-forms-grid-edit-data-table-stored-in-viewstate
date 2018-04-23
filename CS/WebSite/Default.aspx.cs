using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using DevExpress.Web;
using DevExpress.Web.Data;


public partial class _Default : System.Web.UI.Page {
	private DataTable dataTable;

	protected void Page_Load(object sender, EventArgs e) {
		CreateGrid();
	}

	private DataTable CustomDataSourse {
		get {
			if (dataTable != null)
				return dataTable;

			dataTable = ViewState["CustomTable"] as DataTable;
			if (dataTable != null)
				return dataTable;


			dataTable = new DataTable("CustomDTable");
			dataTable.Columns.Add("Id", typeof(Int32));
			dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns[0] };
			dataTable.Columns.Add("Data", typeof(string));

			dataTable.Rows.Add(0, "Data1");
			dataTable.Rows.Add(1, "Data2");
			dataTable.Rows.Add(2, "Data3");
			dataTable.Rows.Add(3, "Data4");
			dataTable.Rows.Add(4, "Data5");
			ViewState["CustomTable"] = dataTable;

			return dataTable;
		}
	}
	protected void grid_DataBinding(object sender, EventArgs e) {
		(sender as ASPxGridView).DataSource = CustomDataSourse;
	}
	protected void grid_DataBound(object sender, EventArgs e) {
		ASPxGridView g = sender as ASPxGridView;
		for (int i = 0; i < g.Columns.Count; i++) {
			GridViewDataTextColumn c = g.Columns[i] as GridViewDataTextColumn;
			if (c == null)
				continue;

			c.PropertiesTextEdit.ValidationSettings.RequiredField.IsRequired = true;
		}

	}


	protected void grid_RowDeleting(object sender, ASPxDataDeletingEventArgs e) {
		int id = (int)e.Keys[0];
		DataRow dr = CustomDataSourse.Rows.Find(id);
		dataTable.Rows.Remove(dr);

		ASPxGridView g = sender as ASPxGridView;
		UpdateData(g);
		e.Cancel = true;
	}
	protected void grid_RowUpdating(object sender, ASPxDataUpdatingEventArgs e) {
		int id = (int)e.OldValues["Id"];
		DataRow dr = CustomDataSourse.Rows.Find(id);
		dr[0] = e.NewValues["Id"];
		dr[1] = e.NewValues["Data"];

		ASPxGridView g = sender as ASPxGridView;
		UpdateData(g);
		g.CancelEdit();
		e.Cancel = true;
	}
	protected void grid_RowInserting(object sender, ASPxDataInsertingEventArgs e) {
		CustomDataSourse.Rows.Add(e.NewValues["Id"], e.NewValues["Data"]);

		ASPxGridView g = sender as ASPxGridView;
		UpdateData(g);
		g.CancelEdit();
		e.Cancel = true;
	}

	private void CreateGrid() {
		ASPxGridView grid = new ASPxGridView();
		grid.ID = "grid";
		this.Form.Controls.Add(grid);

		grid.EnableCallBacks = false;
		grid.KeyFieldName = "Id";

		grid.DataBinding += grid_DataBinding;
		grid.RowDeleting += grid_RowDeleting;
		grid.RowUpdating += grid_RowUpdating;
		grid.RowInserting += grid_RowInserting;
		grid.DataBound += grid_DataBound;
		grid.RowValidating += new ASPxDataValidationEventHandler(grid_RowValidating);
		grid.DataBind();
		if (!this.IsPostBack) {
			GridViewCommandColumn c = new GridViewCommandColumn();
			grid.Columns.Add(c);
            c.ShowEditButton = true;
            c.ShowUpdateButton = true;
            c.ShowNewButtonInHeader = true;
		}

		GridViewDataTextColumn col = grid.Columns["Id"] as GridViewDataTextColumn;
		col.PropertiesTextEdit.ValidationSettings.RegularExpression.ValidationExpression = "\\d{1,9}";
	}

	void grid_RowValidating(object sender, ASPxDataValidationEventArgs e) {        
		int id = (int)e.NewValues["Id"];
		if ((!e.OldValues.Contains("Id") || ((int)e.OldValues["Id"] != id))
			&& (CustomDataSourse.Rows.Find(id) != null)) {
			ASPxGridView grid = sender as ASPxGridView;
			e.Errors[grid.Columns["Id"]] = String.Format("Column 'Id' is constrained to be unique.  Value '{0}' is already present.", id);
		}
	}
	private void UpdateData(ASPxGridView g) {
		ViewState["CustomTable"] = dataTable;
		g.DataBind();
	}
}
