using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class _Default : System.Web.UI.Page
{
    public steppingStone ss;
    protected void Page_Load(object sender, EventArgs e)
    {
        if(IsPostBack)
        {
            if(btnInput1.Enabled==false)
            {
                initForm();
            }
        }
        else
        {
            int i = 0;
        }       
    }

    void initForm()
    {
        int matrixRows, matrixColumns = 0;
        matrixRows = Convert.ToInt32(txbFactories.Text.ToString());
        matrixColumns = Convert.ToInt32(txbWarehouses.Text.ToString());
        int[,] cost = new int[matrixRows, matrixColumns];
        int[] warehouses = new int[matrixColumns];
        int[] factories = new int[matrixRows];
        if(!string.IsNullOrEmpty(hdnCost.Value.ToString() ))
        {   
            //we have user input for matrix pushed into hdn fields  
            int[] intCost = Array.ConvertAll(hdnCost.Value.ToString().Split(','), int.Parse);
            int intCostCounter = 0;
            for (int i = 0; i < matrixRows; i++)
            {
                for (int j = 0; j < matrixColumns; j++)
                {
                    cost[i, j] = intCost[intCostCounter];
                    intCostCounter = intCostCounter + 1;
                }
            }
            warehouses = Array.ConvertAll(hdnWarehouses.Value.ToString().Split(','), int.Parse);
            factories = Array.ConvertAll( hdnFactories.Value.ToString().Split(','), int.Parse);
            ss = new steppingStone(matrixRows, matrixColumns, true, factories, warehouses, cost);
        }
        else
        {
            ss = new steppingStone(matrixRows, matrixColumns, false, factories, warehouses, cost);
        }
        
        outputDiv1.Controls.AddAt(0, ss.displayMatrix);
               
    }

    protected void btnInput1_Click(object sender, EventArgs e)
    {
        initForm();
        String scriptText = "";
        scriptText += "$(document).ready(function () { MakeMatrixEditable(); });";
        Page.ClientScript.RegisterClientScriptBlock(GetType(), "scriptToCall_MakeMatrixEditable", scriptText, true);
        btnInput1.Visible = false;
        btnInput2.Visible = true;
    }
    protected void btnReset_Click(object sender, EventArgs e)
    {
        Response.Redirect("default.aspx");
    }

    protected void btnInput2_Click(object sender, EventArgs e)
    {        
        btnInput2.Visible = false;               
        initForm();
    }
}

public class steppingStone
{
    public int[,] quantity;
    public int[,] cost;

    public int[] factories;
    public int[] warehouses;
    

    int rows;
    int columns;

    public HtmlTable displayMatrix;

    public steppingStone( int mRows, int mColumns, bool useParameters,int[] argfactories, int[] argwarehouses, int[,] argcosts)
    {
        int matrixRows = mRows;
        int matrixColumns = mColumns;

        quantity = new int[matrixRows, matrixColumns];
        cost = new int[matrixRows, matrixColumns];
        
        columns = quantity.GetLength(1);
        rows = quantity.GetLength(0);

        initArray(cost);
        initArray(quantity);

        if (useParameters)
        {
            factories = argfactories;
            warehouses = argwarehouses;
            cost = argcosts;
        }

        
        displayMatrix = buidMatrix();
        
    }


    private void initArray(int[,] intArray)
    {
       
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                intArray[i, j] = 0;
            }
        }        
    }
    
    private int getRowSum(int row)
    {
        int rowSum = 0;       
        for (int i = 0; i < columns; i++)
        {
            rowSum = rowSum + quantity[row, i];
        }
        
        return rowSum;
    }
    private int getColumnSum(int column)
    {
        int columnSum = 0;
        for (int i = 0; i < rows; i++)
        {
            columnSum = columnSum + quantity[i, column];
        }

        return columnSum;
    }

    private int getSum()
    {
        int sum = 0;
        for(int i=0; i<rows; i++)
        {
            for(int j=0; j<columns; j++)
            {
                sum = sum + quantity[i, j];
            }
        }
        return sum;
    }

    private HtmlTable buidMatrix()
    {
        HtmlTable tbl = new HtmlTable();
        int rowCount = rows + 2;
        for (int i = 0; i < rowCount; i++)
        {
            HtmlTableRow tblRow = new HtmlTableRow();
            int columnCount = (columns * 2) + 2;
            for (int j = 0; j < columnCount; j++)
            {
                HtmlTableCell tblCell = new HtmlTableCell();
                int quantityCol = ((j + 1) / 2) - 1;
                int costCol = (j / 2) - 1;

                //header row
                if(i==0) 
                {
                    if (j == 0)
                    {
                        tblCell.InnerText = "From/To";
                    }
                    if (j > 0 && j < columnCount && IsOdd(j))
                    {
                        tblCell.ColSpan = 2;
                        int location = ((j + 1) / 2);
                        tblCell.InnerText = "Warehouse " + location;
                    }
                    if (j + 1 == columnCount)
                    {
                        tblCell.InnerText = "Factory Capacity";
                    }
                }
                
                //data rows
                if (i > 0 && i + 1 < rowCount)
                {
                    int factory = i;
                    if (j == 0)
                    {
                        tblCell.InnerText = "Factory " + factory;
                    }
                    if (j > 0 && j + 1 < columnCount)
                    {
                        if (IsOdd(j))
                        {

                            tblCell.InnerText = quantity[i - 1, quantityCol].ToString();
                        }
                        else
                        {
                            tblCell.InnerText = cost[i - 1, costCol].ToString();
                            tblCell.Attributes.Add("class", "editable costs");
                        }
                    }
                    if(j+1==columnCount)
                    {
                        tblCell.InnerText = getRowSum(i - 1).ToString();
                        tblCell.Attributes.Add("class", "editable factories");
                    }
                }

                //footer rows
                if (i + 1 == rowCount)
                {
                    if (j == 0)
                    {
                        tblCell.InnerText = "Warehouse Requirements";
                    }
                    if (j > 0 && j + 1 < columnCount && IsOdd(j))
                    {
                        tblCell.InnerText = getColumnSum(quantityCol).ToString();
                        tblCell.Attributes.Add("class", "editable warehouses");
                    }
                    if (j + 1 == columnCount)
                    {
                        tblCell.InnerText = getSum().ToString();
                    }
                }

                if (i == 0 && IsOdd(j) && j>0 && j<columnCount)
                {
                    j = j + 1; //skip adding unwanted columns
                }
                tblRow.Cells.Add(tblCell);
            }
            tbl.Rows.Add(tblRow);
        }
        return tbl;
    }


    private void initialAllocation()
    {

    }

    public static bool IsOdd(int value)
    {
        return value % 2 != 0;
    }
}