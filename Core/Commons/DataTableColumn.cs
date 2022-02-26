/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting                       Component : Root Types                              *
*  Assembly : Empiria.FinancialAccounting.dll            Pattern   : Information Holder                      *
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


    public string Field {
      get;
    }


    public string Title {
      get;
    }


    public string Type {
      get;
    }


    public int Digits {
      get;
    }

  }  // class DataTableColumn

}  // namespace Empiria.FinancialAccounting
