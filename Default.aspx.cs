﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class _Default : System.Web.UI.Page
{
    public steppingStone ss;
    public string stepHTML = "<br/><br/><div class='stepsDiv'><p class='stepsPara'> {0} </p></div><br/><br/>";
    public string lastOutput;
    protected void Page_Load(object sender, EventArgs e)
    {
        ClientScript.GetPostBackEventReference(this, string.Empty);
        mainDiv.Controls.AddAt(0, new LiteralControl(string.Format(stepHTML, "1. Enter matrix size:")));
        if (IsPostBack)
        {
            //Anything we output dynamically to the "outputDiv1" is passed back as markup so it could be redisplayed and won't have to regenerate. 
            string parameter = Request["__EVENTARGUMENT"]; // parameter
            string postSender = Request["__EVENTTARGET"]; // btnSave
            lastOutput = parameter;
            outputDiv1.Controls.Clear();
            outputDiv1.Controls.Add(new LiteralControl(parameter));
        }
        else
        {
            btnInput1.Visible = true;
        }
    }

    void initForm(int matrixNumber)
    {
        int matrixRows, matrixColumns = 0;
        matrixRows = Convert.ToInt32(txbFactories.Text.ToString());
        matrixColumns = Convert.ToInt32(txbWarehouses.Text.ToString());
        int[,] cost = new int[matrixRows, matrixColumns];
        int[] warehouses = new int[matrixColumns];
        int[] factories = new int[matrixRows];
        string matrixID = "Matrix" + matrixNumber;
        bool useUserInput = false;
        if (matrixNumber > 1)
        {
            //we have user input for matrix pushed into hdn fields  (on second submit button - after getting user input in matrix for cost and quantities)
            useUserInput = true;
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
            factories = Array.ConvertAll(hdnFactories.Value.ToString().Split(','), int.Parse);
        }

        ss = new steppingStone(matrixRows, matrixColumns, useUserInput, factories, warehouses, cost, matrixID);
    }


    protected void btnInput1_Click(object sender, EventArgs e)
    {
        initForm(1);
        String scriptText = "";
        scriptText += "$(document).ready(function () { MakeMatrixEditable(); });";
        Page.ClientScript.RegisterClientScriptBlock(GetType(), "scriptToCall_MakeMatrixEditable", scriptText, true);
        outputDiv1.Controls.Add(ss.buidMatrix(false));
        btnInput1.Visible = false;
        btnInput2.Visible = true;
        txbFactories.ReadOnly = true;
        txbWarehouses.ReadOnly = true;
    }


    protected void btnInput2_Click(object sender, EventArgs e)
    {
        btnInput2.Visible = false;
        btnInput3.Visible = true;
        String scriptText = "";
        scriptText += "$(document).ready(function () { RemoveFirstMatrix(); });";
        Page.ClientScript.RegisterClientScriptBlock(GetType(), "Script for deleting Matrix1", scriptText, true);
        outputDiv1.Controls.Add(new LiteralControl(string.Format(stepHTML, "2. Initialize Matrix:")));
        initForm(2);
        outputDiv1.Controls.Add(ss.buidMatrix(true));
    }

    protected void btnInput3_Click(object sender, EventArgs e)
    {
        outputDiv1.Controls.Add(new LiteralControl(string.Format(stepHTML, "3. Initial allocation done by using Vogel's Approximation Method: ")));
        HtmlTable firstSolutionMatrix = new HtmlTable();
        initForm(3);
        ss.initialAllocation();
        outputDiv1.Controls.Add(ss.buidMatrix(true));
        btnInput3.Visible = true;
    }
    protected void btnReset_Click(object sender, EventArgs e)
    {
        Response.Redirect("default.aspx");
    }
}

public class steppingStone
{
    public int[,] quantity;
    public int[,] cost;

    //Required demand and supply
    public int[] factories;
    public int[] warehouses;

    //Current demand and supply
    public int[] currentFactories;
    public int[] currentWarehouses;

    //variables for initial allocation tracking
    bool[] demandExhaustStatus;
    bool[] supplyExhaustStatus;
    public bool hasInputParameters;

    int rows;
    int columns;

    string matrixID;

    public HtmlTable displayMatrix;

    public steppingStone(int mRows, int mColumns, bool useParameters, int[] argfactories, int[] argwarehouses, int[,] argcosts, string argMatrixID)
    {
        int matrixRows = mRows;
        int matrixColumns = mColumns;
        matrixID = argMatrixID;

        quantity = new int[matrixRows, matrixColumns];
        cost = new int[matrixRows, matrixColumns];

        columns = quantity.GetLength(1);
        rows = quantity.GetLength(0);

        initArray(cost);
        initArray(quantity);

        demandExhaustStatus = new bool[columns];
        supplyExhaustStatus = new bool[rows];
        initBoolArray(demandExhaustStatus, false);
        initBoolArray(supplyExhaustStatus, false);

        hasInputParameters = useParameters;
        if (useParameters)
        {
            factories = argfactories.Clone() as int[];
            currentFactories = argfactories.Clone() as int[];

            warehouses = argwarehouses.Clone() as int[];
            currentWarehouses = argwarehouses.Clone() as int[];

            cost = argcosts;
        }
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

    private int getRowSum(int row, int[,] array)
    {
        int rowSum = 0;
        for (int i = 0; i < columns; i++)
        {
            rowSum = rowSum + array[row, i];
        }

        return rowSum;
    }
    private int getColumnSum(int column, int[,] array)
    {
        int columnSum = 0;
        for (int i = 0; i < rows; i++)
        {
            columnSum = columnSum + array[i, column];
        }

        return columnSum;
    }

    private int getSum(int[,] array)
    {
        int sum = 0;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                sum = sum + array[i, j];
            }
        }
        return sum;
    }

    //Builds a html table to represent user input. Quantity array values and cost array values are used.
    //useInputData arg determines if the table should have total supply for  each factory and total demand by each warehouse based on user input or is based on current array values.
    public HtmlTable buidMatrix(bool useInputData)
    {
        HtmlTable tbl = new HtmlTable();
        tbl.ID = matrixID;
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
                if (i == 0)
                {
                    tblCell.Attributes.Add("class", "headerCell");
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
                        tblCell.InnerText = "Factory Supply";
                    }
                }

                //data rows
                if (i > 0 && i + 1 < rowCount)
                {
                    int factory = i;
                    if (j == 0)
                    {
                        tblCell.InnerText = "Factory " + factory;
                        tblCell.Attributes.Add("class", "factoryCell");
                    }
                    if (j > 0 && j + 1 < columnCount)
                    {
                        if (IsOdd(j))
                        {

                            tblCell.InnerText = quantity[i - 1, quantityCol].ToString();
                            if (quantity[i - 1, quantityCol] > 0)
                                tblCell.Attributes.Add("class", "allocatedCell");
                            else
                                tblCell.Attributes.Add("class", "unallocatedCell");
                        }
                        else
                        {
                            tblCell.InnerText = cost[i - 1, costCol].ToString();
                            tblCell.Attributes.Add("class", "editable costs costCells");
                        }
                    }
                    if (j + 1 == columnCount)
                    {
                        if (useInputData)
                        {
                            tblCell.InnerText = factories[i - 1].ToString();
                            tblCell.Attributes.Add("class", "factoryCell");
                        }
                        else
                        {
                            tblCell.InnerText = getRowSum(i - 1, quantity).ToString();
                            tblCell.Attributes.Add("class", "editable factories factoryCell");
                        }


                    }
                }

                //footer rows
                if (i + 1 == rowCount)
                {
                    if (j == 0)
                    {
                        tblCell.InnerText = "Warehouse Demand";
                        tblCell.Attributes.Add("class", "warehousCell");
                    }
                    if (j > 0 && j + 1 < columnCount && IsOdd(j))
                    {
                        if (useInputData)
                        {
                            tblCell.InnerText = warehouses[quantityCol].ToString();
                            tblCell.Attributes.Add("class", "warehousCell");
                        }
                        else
                        {
                            tblCell.InnerText = getColumnSum(quantityCol, quantity).ToString();
                            tblCell.Attributes.Add("class", "editable warehouses warehousCell");
                        }

                    }
                    else if ((j > 0 && j + 1 < columnCount && !IsOdd(j)))
                    {
                        tblCell.Attributes.Add("class", "costCells");
                        tblCell.InnerText = getColumnSum(costCol, cost).ToString();
                    }
                    if (j + 1 == columnCount)
                    {

                        if (useInputData)
                        {
                            tblCell.InnerText = factories.Sum().ToString();
                        }
                        else
                        {
                            tblCell.InnerText = getSum(quantity).ToString();
                        }
                        tblCell.Attributes.Add("class", "grandTotalCell");
                    }
                }

                if (i == 0 && IsOdd(j) && j > 0 && j < columnCount)
                {
                    j = j + 1; //skip adding unwanted columns
                }
                tblRow.Cells.Add(tblCell);
            }
            tbl.Rows.Add(tblRow);
        }
        return tbl;
    }


    public void initialAllocation()
    {

        //Using vogel approximation method to do find an inital solution
        int allocationRowIndex = -1;
        int allocationColIndex = -1;
        bool recall = true;

        #region Calculating row differences for cost
        //find rows difference - take two minimum cost values of every row

        int?[] rowsDiff = new int?[rows];
        for (int i = 0; i < rows; i++)
        {
            //firstSmallest variable holds the smallest cost value and secondSmallest holds the next to smallest cost value
            if (!supplyExhaustStatus[i])//skip exausted rows
            {
                int firstSmallest = 0; int secondSmallest = 0; bool useSecondSmallest = false;
                bool firstDone = false; bool secondDone = false;
                for (int j = 0; j < columns; j++)
                {
                    if (!demandExhaustStatus[j])
                    {
                        if (!firstDone)
                        {
                            firstSmallest = cost[i, j];
                            firstDone = true;
                        }
                        else if (firstDone && !secondDone && !useSecondSmallest)
                        {
                            if (cost[i, j] < firstSmallest)
                            {
                                secondSmallest = firstSmallest;
                                firstSmallest = cost[i, j];
                            }
                            else
                            {
                                secondSmallest = cost[i, j];
                            }
                            secondDone = true;
                            useSecondSmallest = true;
                        }
                        else
                        {
                            if (secondSmallest < firstSmallest)
                            {
                                int temp = firstSmallest;
                                firstSmallest = secondSmallest;
                                secondSmallest = temp;
                            }

                            if (cost[i, j] < firstSmallest)
                            {
                                secondSmallest = firstSmallest;
                                firstSmallest = cost[i, j];
                            }
                            else if (cost[i, j] < secondSmallest)
                            {
                                secondSmallest = cost[i, j];
                            }
                        }
                    }
                }
                if (useSecondSmallest)
                {

                    rowsDiff[i] = secondSmallest - firstSmallest;
                }
                else
                {
                    rowsDiff[i] = firstSmallest;
                }
            }
            else
            {
                rowsDiff[i] = null;
            }
        }
        #endregion

        #region Calculating column differences for cost
        //find rows difference - take two minimum cost values of every row

        int?[] columnsDiff = new int?[columns];
        for (int i = 0; i < columns; i++)
        {
            if (!demandExhaustStatus[i])//skip exausted columns
            {
                //firstSmallest variable holds the smallest cost value and secondSmallest holds the next to smallest cost value
                int firstSmallest = 0; int secondSmallest = 0; bool useSecondSmallest = false;
                bool firstDone = false; bool secondDone = false;
                for (int j = 0; j < rows; j++)
                {
                    if (!supplyExhaustStatus[j])
                    {
                        if (!firstDone)
                        {
                            firstSmallest = cost[j, i];
                            firstDone = true;
                        }
                        else if (firstDone && !secondDone && !useSecondSmallest)
                        {
                            if (cost[j, i] < firstSmallest)
                            {
                                secondSmallest = firstSmallest;
                                firstSmallest = cost[j, i];
                            }
                            else
                            {
                                secondSmallest = cost[j, i];
                            }
                            secondDone = true;
                            useSecondSmallest = true;
                        }
                        else
                        {
                            if (secondSmallest < firstSmallest)
                            {
                                int temp = firstSmallest;
                                firstSmallest = secondSmallest;
                                secondSmallest = temp;
                            }
                            if (cost[j, i] < firstSmallest)
                            {
                                secondSmallest = firstSmallest;
                                firstSmallest = cost[j, i];
                            }
                            else if (cost[j, i] < secondSmallest)
                            {
                                secondSmallest = cost[j, i];
                            }
                        }
                    }
                }
                if (useSecondSmallest)
                {
                    columnsDiff[i] = secondSmallest - firstSmallest;
                }
                else
                {
                    columnsDiff[i] = firstSmallest;
                }
            }
            else
            {
                columnsDiff[i] = null;
            }
        }
        #endregion

        //Finding max value in row difference array
        int? maxRowDiff = rowsDiff.Max();

        //Finding max value in col difference array
        int? maxColDiff = columnsDiff.Max();

        #region use row index at which this value was located and find column in this row that has smallest cost value
        if (maxRowDiff >= maxColDiff)
        {
            int maxRowValueIndex = Array.IndexOf(rowsDiff, maxRowDiff);
            int minColumnIndex = 0;

            int minColValue = 0;
            bool firstDone = false;
            for (int i = 0; i < columns; i++)
            {
                if (!demandExhaustStatus[i])
                {
                    if (!firstDone)
                    {
                        minColValue = cost[maxRowValueIndex, i];
                        minColumnIndex = i;
                        firstDone = true;
                    }
                    else if (cost[maxRowValueIndex, i] < minColValue)
                    {
                        minColValue = cost[maxRowValueIndex, i];
                        minColumnIndex = i;
                    }
                }
            }
            allocationRowIndex = maxRowValueIndex;
            allocationColIndex = minColumnIndex;
        }
        #endregion

        #region use col index at which this value was located and find row in this column that has smallest cost value
        else
        {
            int maxColValueIndex = Array.IndexOf(columnsDiff, maxColDiff);
            int minRowsIndex = 0;

            int minRowValue = 0;
            bool firstDone = false;
            for (int i = 0; i < rows; i++)
            {
                if (!supplyExhaustStatus[i])
                {
                    if (!firstDone)
                    {
                        minRowValue = cost[i, maxColValueIndex];
                        minRowsIndex = i;
                        firstDone = true;
                    }
                    else if (cost[i, maxColValueIndex] < minRowValue)
                    {
                        minRowValue = cost[i, maxColValueIndex];
                        minRowsIndex = i;
                    }
                }
            }
            allocationRowIndex = minRowsIndex;
            allocationColIndex = maxColValueIndex;
        }
        #endregion

        #region Allocating
        //now we know which element we have to allocate in quantity array. 
        //We should determine how much we can allocate based on supply and demand
        int initialSupply = factories[allocationRowIndex];
        int initialDemand = warehouses[allocationColIndex];

        int currentSupply = currentFactories[allocationRowIndex];
        int currentDemand = currentWarehouses[allocationColIndex];

        int currentAllocation = quantity[allocationRowIndex, allocationColIndex];

        if (currentAllocation < currentDemand && currentSupply > 0)
        {
            //How much we can allocate
            int requiredDemand = currentDemand - currentAllocation;
            int availableSupply = currentSupply - currentAllocation;

            int allocate = 0, remainingSupply = 0, remainingDemand = 0;
            if (requiredDemand < availableSupply)
            {
                allocate = requiredDemand;
            }
            else if (requiredDemand >= availableSupply)
            {
                allocate = availableSupply;
            }
            remainingSupply = currentSupply - allocate;
            remainingDemand = currentDemand - allocate;
            quantity[allocationRowIndex, allocationColIndex] = allocate;
            currentFactories[allocationRowIndex] = remainingSupply;
            currentWarehouses[allocationColIndex] = remainingDemand;
            if (remainingDemand == 0)
            {
                demandExhaustStatus[allocationColIndex] = true;
            }
            if (remainingSupply == 0)
            {
                supplyExhaustStatus[allocationRowIndex] = true;
            }
        }
        else if (currentAllocation == currentDemand)
        {
            //don't allocate
            demandExhaustStatus[allocationColIndex] = true;
        }
        else if (currentSupply == 0)
        {
            supplyExhaustStatus[allocationRowIndex] = true;
        }

        #endregion

        #region When to terminate recursion call
        if (getSum(quantity) == factories.Sum())
        {
            recall = false;
        }

        if (recall)
        {
            initialAllocation();
        }
        #endregion
    }

    private bool IsOdd(int value)
    {
        return value % 2 != 0;
    }

    private void initBoolArray(bool[] arr, bool value)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = value;
        }
    }
}
