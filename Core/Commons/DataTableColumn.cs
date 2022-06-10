/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting                       Component : Common Types                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Information Holder                      *
*  Type     : DataTableColumn                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a data table column.                                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting {

  /// <summary>Describes a data table column.</summary>
  public class DataTableColumn {

    public DataTableColumn(string field, string title, string type, int digits = 2) {

      this.Field = field;
      this.Title = title;
      this.Type = type;

      if (type == "decimal") {
        this.Digits = digits;
      }
    }

    public string Column {
      get; set;
    }


    public string Title {
      get; set;
    }

    public string Field {
      get; set;
    }

    public string Type {
      get; set;
    }

    public int Digits {
      get; set;
    }

  }  // class DataTableColumn

}  // namespace Empiria.FinancialAccounting
