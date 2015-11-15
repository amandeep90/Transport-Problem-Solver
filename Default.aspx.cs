using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        steppingStone ss = new steppingStone();
        ss.init();
        mainDiv.Controls.Add(ss.buidMatrix());
               
        
    }
}

public class steppingStone
{
    public int[,] quantity;
    public int[,] cost;

    int rows;
    int columns;

    public steppingStone()
    {
        quantity= new int [3,3];
        cost = new int[3, 3];
    }

    public void init()
    {
        cost[0, 0] = 5;
        cost[0, 1] = 4;
        cost[0, 2] = 3;
        cost[1, 0] = 8;
        cost[1, 1] = 4;
        cost[1, 2] = 3;
        cost[2, 0] = 9;
        cost[2, 1] = 7;
        cost[2, 2] = 5;

        quantity[0, 0] = 100;
        quantity[0, 1] = 0;
        quantity[0, 2] = 0;
        quantity[1, 0] = 200;
        quantity[1, 1] = 100;
        quantity[1, 2] = 0;
        quantity[2, 0] = 0;
        quantity[2, 1] = 100;
        quantity[2, 2] = 200;

        columns = quantity.GetLength(1);
        rows = quantity.GetLength(0);
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

    public HtmlTable buidMatrix()
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
                if(i==0 && j==0)
                {
                    tblCell.InnerText = "From/To";
                }
                if (i == 0 && j > 0 && j < columnCount && IsOdd(j))
                {
                    tblCell.ColSpan = 2;
                    int location = ((j+1)/2);
                    tblCell.InnerText = "Warehouse " + location;
                }
                if(i==0 && j+1 == columnCount)
                {
                    tblCell.InnerText = "Factory Capacity";
                }   
                if(i>0 && i+1<rowCount)
                {
                    int factory = i;
                    if(j==0)
                    {
                        tblCell.InnerText = "Factory " + factory;                    
                    }
                    if(j > 0 && j+1 < columnCount)
                    {
                        if(IsOdd(j))
                        {
                            
                            tblCell.InnerText = quantity[i - 1, quantityCol].ToString();
                        }
                        else
                        {                            
                            tblCell.InnerText = cost[i - 1, costCol].ToString();
                        }
                    }
                }
                if(i+1==rowCount)
                {
                    if(j==0)
                    {
                        tblCell.InnerText = "Warehouse Requirements";
                    }
                    if (j > 0 && j + 1 < columnCount && IsOdd(j))
                    {
                        tblCell.InnerText = getColumnSum(quantityCol).ToString ();
                    }
                    if(j+1 == columnCount)
                    {
                        tblCell.InnerText = getSum().ToString();
                    }
                }
                
                tblRow.Cells.Add(tblCell);
            }
            tbl.Rows.Add(tblRow);
        }
        return tbl;
    }

    public static bool IsOdd(int value)
    {
        return value % 2 != 0;
    }
}