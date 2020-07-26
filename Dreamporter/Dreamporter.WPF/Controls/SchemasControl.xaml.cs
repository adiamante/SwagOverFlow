using Dreamporter.Core;
using SwagOverFlow.Clients;
using SwagOverFlow.WPF.Controls;
using SwagOverFlow.WPF.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Dreamporter.WPF.Controls
{
    /// <summary>
    /// Interaction logic for SchemasControl.xaml
    /// </summary>
    public partial class SchemasControl : SwagControlBase
    {
        #region Schemas
        public static DependencyProperty SchemasProperty =
            DependencyProperty.Register(
                "Schemas",
                typeof(ICollection<Schema>),
                typeof(SchemasControl));

        public ICollection<Schema> Schemas
        {
            get { return (ICollection<Schema>)GetValue(SchemasProperty); }
            set { SetValue(SchemasProperty, value); }
        }
        #endregion Schemas
        #region Save
        public static readonly RoutedEvent SaveEvent =
            EventManager.RegisterRoutedEvent(
            "Save",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(SchemasControl));

        public event RoutedEventHandler Save
        {
            add { AddHandler(SaveEvent, value); }
            remove { RemoveHandler(SaveEvent, value); }
        }
        #endregion Save
        #region ShowSaveButton
        public static DependencyProperty ShowSaveButtonProperty =
            DependencyProperty.Register(
                "ShowSaveButton",
                typeof(Boolean),
                typeof(SchemasControl),
                new PropertyMetadata(false));

        public Boolean ShowSaveButton
        {
            get { return (Boolean)GetValue(ShowSaveButtonProperty); }
            set
            {
                SetValue(ShowSaveButtonProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion ShowSaveButton

        #region Initialization
        public SchemasControl()
        {
            InitializeComponent();
        }
        #endregion Initialization

        #region Events
        private void SwagItemsControl_Add(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)e.OriginalSource;

            switch (fe.DataContext)
            {
                case Schema schema:
                    schema.Tables.Add(new SchemaTable());
                    Refresh(schema.Tables);
                    break;
                case SchemaTable table:
                    table.Columns.Add(new SchemaColumn());
                    Refresh(table.Columns);
                    break;
                case SchemaColumn column:
                    TreeViewItem tviCol = SwagItemsControl.ContainerFromItemRecursive(column);
                    TreeViewItem tviParentTable = tviCol.TryFindParent<TreeViewItem>();
                    SchemaTable tblParent = (SchemaTable)tviParentTable.DataContext;
                    tblParent.Columns.Add(new SchemaColumn());
                    Refresh(tblParent.Columns);
                    break;
                default:
                    Schemas.Add(new Schema());
                    Refresh(Schemas);
                    break;
            }
        }

        private void SwagItemsControl_Remove(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)e.OriginalSource;

            switch (fe.DataContext)
            {
                case Schema schema:
                    Schemas.Remove(schema);
                    Refresh(Schemas);
                    break;
                case SchemaTable table:
                    TreeViewItem tviTable = SwagItemsControl.ContainerFromItemRecursive(table);
                    TreeViewItem tviParentSchema = tviTable.TryFindParent<TreeViewItem>();
                    Schema schParent = (Schema)tviParentSchema.DataContext;
                    schParent.Tables.Remove(table);
                    Refresh(schParent.Tables);
                    break;
                case SchemaColumn column:
                    TreeViewItem tviCol = SwagItemsControl.ContainerFromItemRecursive(column);
                    TreeViewItem tviParentTable = tviCol.TryFindParent<TreeViewItem>();
                    SchemaTable tblParent = (SchemaTable)tviParentTable.DataContext;
                    tblParent.Columns.Remove(column);
                    Refresh(tblParent.Columns);
                    break;
            }
        }

        private void SwagItemsControl_Save(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(SaveEvent));
        }

        private void Table_GenerateCTE_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)e.OriginalSource;
            SchemaTable table = (SchemaTable)fe.DataContext;

            String alias = table.Schema.Name.Length > 0 ? Char.ToLower(table.Schema.Name[0]).ToString() : "";

            for (int i = 0; i < table.Name.Length; i++)
            {
                Char c = table.Name[i];
                if (i == 0 && Char.IsLetter(c))
                {
                    alias += Char.ToLower(c);
                }
                else if (c == '.' && i + 1 < table.Name.Length)
                {
                    i++;
                    c = table.Name[i];
                    alias += Char.ToLower(c);
                }
                else if (Char.IsUpper(c))
                {
                    alias += Char.ToLower(c);
                }
            }

            if (alias == "or")
            {
                alias = "or0";
            }

            StringBuilder sb = new StringBuilder();
            sb.Append($"WITH {table.Name} (");

            foreach (SchemaColumn column in table.Columns)
            {
                sb.Append($"{column.Name}, ");
            }

            //Remove last two characters
            sb.Length--;    //removes space
            sb.Length--;    //removes last comma

            sb.Append(") AS\n");
            sb.Append("(\n");
            sb.Append("\tSELECT ");

            foreach (SchemaColumn column in table.Columns)
            {
                sb.Append($"{alias}.{column.Name}, ");
            }

            //Remove last two characters
            sb.Length--;    //removes space
            sb.Length--;    //removes last comma

            sb.Append("\n");
            sb.Append($"\tFROM [{table.Schema.Name}.{table.Name}] {alias}\n");
            sb.Append(")\n");
            sb.Append("SELECT *\n");
            sb.Append($"FROM {table.Name}");

            UIHelper.StringInputDialog($"Generated CTE for {table.Schema.Name}.{table.Name}", sb.ToString());

        }

        private void Schema_GenerateSummary_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)e.OriginalSource;
            Schema schema = (Schema)fe.DataContext;
            StringBuilder sb = new StringBuilder();
            
            foreach (SchemaTable table in schema.Tables)
            {
                String numericColumn = "0";
                
                //Find last numeric column and that will be summed as Total
                foreach (SchemaColumn column in table.Columns)
                {
                    if (column.DataType == SchemaColumnDataType.Integer || column.DataType == SchemaColumnDataType.Real)
                    {
                        numericColumn = column.Name;
                    }
                }

                sb.AppendLine($"SELECT '{table.Name}' AS [Table], COUNT(*) AS Count, SUM({numericColumn}) AS Total, '{numericColumn}' AS TotalCriteria");
                sb.AppendLine($"FROM [{schema.Name}.{table.Name}]");
                sb.AppendLine($"UNION ALL");
            }

            sb.Length -= 13;    //Remove last Union All

            UIHelper.StringInputDialog($"Generated CTE Summary {schema.Name}", sb.ToString());
        }
        #endregion Events

        #region Method
        private void Refresh(Object col)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(col);
            view.Refresh();
        }

        private void SwagItemsControl_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (String filePath in files)
                {
                    try
                    {
                        SqliteClient client = new SqliteClient($"Data Source={filePath};Version=3;");
                        client.OpenConnection();
                        DataSet ds = client.GetDataSet();

                        foreach (DataTable dtbl in ds.Tables)
                        {
                            String schemaName = "", tableName = "";
                            String[] parts = dtbl.TableName.Split('.');   //period delimeter
                            if (parts.Length == 2)  //schema + table
                            {
                                schemaName = parts[0];
                                tableName = parts[1];
                            }
                            else if (parts.Length == 1)     //just table
                            {
                                tableName = parts[0];
                            }

                            if (schemaName.ToLower() == "util")     //skip util because it's automatically generated
                            {
                                continue;
                            }

                            Schema schema = Schemas.FirstOrDefault(s => s.Name == schemaName);
                            if (schema == null)
                            {
                                schema = new Schema() { Name = schemaName };
                                Schemas.Add(schema);
                            }

                            SchemaTable table = schema.Tables.FirstOrDefault(t => t.Name == tableName);
                            if (table == null)
                            {
                                table = new SchemaTable() { Name = tableName };
                                schema.Tables.Add(table);
                            }

                            SqliteHelper.IterateColumns(dtbl, new Action<DataColumn, string>((dc, colType) =>
                            {
                                SchemaColumn column = table.Columns.FirstOrDefault(c => c.Name == dc.ColumnName);
                                if (column == null)
                                {
                                    column = new SchemaColumn() { Name = dc.ColumnName };
                                    switch (colType)
                                    {
                                        case "INTEGER":
                                            column.DataType = SchemaColumnDataType.Integer;
                                            break;
                                        case "REAL":
                                            column.DataType = SchemaColumnDataType.Real;
                                            break;
                                        case "DATETIME":
                                            column.DataType = SchemaColumnDataType.DateTime;
                                            break;
                                        case "VARCHAR(255)":
                                        default:
                                            column.DataType = SchemaColumnDataType.String;
                                            break;
                                    }
                                    table.Columns.Add(column);
                                }
                            }));
                        }

                        client.CloseConnection();
                        Refresh(Schemas);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to parse schema of {filePath}: {ex.Message}");
                    }
                }
            }
        }

        #endregion Method

    }
}
