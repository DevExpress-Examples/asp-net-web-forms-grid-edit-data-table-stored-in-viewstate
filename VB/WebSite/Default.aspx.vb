Imports System
Imports System.Data
Imports System.Configuration
Imports System.Collections
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports DevExpress.Web
Imports DevExpress.Web.Data


Partial Public Class _Default
	Inherits System.Web.UI.Page

	Private dataTable As DataTable

	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		CreateGrid()
	End Sub

	Private ReadOnly Property CustomDataSourse() As DataTable
		Get
			If dataTable IsNot Nothing Then
				Return dataTable
			End If

			dataTable = TryCast(ViewState("CustomTable"), DataTable)
			If dataTable IsNot Nothing Then
				Return dataTable
			End If


			dataTable = New DataTable("CustomDTable")
			dataTable.Columns.Add("Id", GetType(Int32))
			dataTable.PrimaryKey = New DataColumn() { dataTable.Columns(0) }
			dataTable.Columns.Add("Data", GetType(String))

			dataTable.Rows.Add(0, "Data1")
			dataTable.Rows.Add(1, "Data2")
			dataTable.Rows.Add(2, "Data3")
			dataTable.Rows.Add(3, "Data4")
			dataTable.Rows.Add(4, "Data5")
			ViewState("CustomTable") = dataTable

			Return dataTable
		End Get
	End Property
	Protected Sub grid_DataBinding(ByVal sender As Object, ByVal e As EventArgs)
		TryCast(sender, ASPxGridView).DataSource = CustomDataSourse
	End Sub
	Protected Sub grid_DataBound(ByVal sender As Object, ByVal e As EventArgs)
		Dim g As ASPxGridView = TryCast(sender, ASPxGridView)
		For i As Integer = 0 To g.Columns.Count - 1
			Dim c As GridViewDataTextColumn = TryCast(g.Columns(i), GridViewDataTextColumn)
			If c Is Nothing Then
				Continue For
			End If

			c.PropertiesTextEdit.ValidationSettings.RequiredField.IsRequired = True
		Next i

	End Sub


	Protected Sub grid_RowDeleting(ByVal sender As Object, ByVal e As ASPxDataDeletingEventArgs)
		Dim id As Integer = CInt(Math.Truncate(e.Keys(0)))
		Dim dr As DataRow = CustomDataSourse.Rows.Find(id)
		dataTable.Rows.Remove(dr)

		Dim g As ASPxGridView = TryCast(sender, ASPxGridView)
		UpdateData(g)
		e.Cancel = True
	End Sub
	Protected Sub grid_RowUpdating(ByVal sender As Object, ByVal e As ASPxDataUpdatingEventArgs)
		Dim id As Integer = CInt(Math.Truncate(e.OldValues("Id")))
		Dim dr As DataRow = CustomDataSourse.Rows.Find(id)
		dr(0) = e.NewValues("Id")
		dr(1) = e.NewValues("Data")

		Dim g As ASPxGridView = TryCast(sender, ASPxGridView)
		UpdateData(g)
		g.CancelEdit()
		e.Cancel = True
	End Sub
	Protected Sub grid_RowInserting(ByVal sender As Object, ByVal e As ASPxDataInsertingEventArgs)
		CustomDataSourse.Rows.Add(e.NewValues("Id"), e.NewValues("Data"))

		Dim g As ASPxGridView = TryCast(sender, ASPxGridView)
		UpdateData(g)
		g.CancelEdit()
		e.Cancel = True
	End Sub

	Private Sub CreateGrid()
		Dim grid As New ASPxGridView()
		grid.ID = "grid"
		Me.Form.Controls.Add(grid)

		grid.EnableCallBacks = False
		grid.KeyFieldName = "Id"

		AddHandler grid.DataBinding, AddressOf grid_DataBinding
		AddHandler grid.RowDeleting, AddressOf grid_RowDeleting
		AddHandler grid.RowUpdating, AddressOf grid_RowUpdating
		AddHandler grid.RowInserting, AddressOf grid_RowInserting
		AddHandler grid.DataBound, AddressOf grid_DataBound
		AddHandler grid.RowValidating, AddressOf grid_RowValidating
		grid.DataBind()
		If Not Me.IsPostBack Then
			Dim c As New GridViewCommandColumn()
			grid.Columns.Add(c)
			c.ShowEditButton = True
			c.ShowUpdateButton = True
			c.ShowNewButtonInHeader = True
		End If

		Dim col As GridViewDataTextColumn = TryCast(grid.Columns("Id"), GridViewDataTextColumn)
		col.PropertiesTextEdit.ValidationSettings.RegularExpression.ValidationExpression = "\d{1,9}"
	End Sub

	Private Sub grid_RowValidating(ByVal sender As Object, ByVal e As ASPxDataValidationEventArgs)
		Dim id As Integer = CInt(Math.Truncate(e.NewValues("Id")))
		If (Not e.OldValues.Contains("Id") OrElse (CInt(Math.Truncate(e.OldValues("Id"))) <> id)) AndAlso (CustomDataSourse.Rows.Find(id) IsNot Nothing) Then
			Dim grid As ASPxGridView = TryCast(sender, ASPxGridView)
			e.Errors(grid.Columns("Id")) = String.Format("Column 'Id' is constrained to be unique.  Value '{0}' is already present.", id)
		End If
	End Sub
	Private Sub UpdateData(ByVal g As ASPxGridView)
		ViewState("CustomTable") = dataTable
		g.DataBind()
	End Sub
End Class
