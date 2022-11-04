/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting                       Component : Common Types                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Information Holder                      *
*  Type     : DataTableColumn                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a data table column.                                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Json;

namespace Empiria.FinancialAccounting {

  /// <summary>Describes a data table column.</summary>
  public class DataTableColumn {

    #region Constructors and parsers

    private DataTableColumn() {
      // no-op
    }


    public DataTableColumn(string field, string title, string type, int digits = 2) {
      this.Field = field;
      this.Title = title;
      this.Type = type;

      if (type == "decimal") {
        this.Digits = digits;
      }
    }


    static public DataTableColumn Parse(JsonObject json) {
      Assertion.Require(json, nameof(json));

      return new DataTableColumn {
        Field = json.Get<string>("field"),
        Title = json.Get<string>("title"),
        Type = json.Get<string>("type"),
        Digits = json.Get<int>("digits", 2),
        Column = json.Get<string>("column", string.Empty),
        Hidden = json.Get<bool>("hidden", false)
      };
    }


    #endregion Constructors and parsers

    #region Properties

    public string Field {
      get; private set;
    }


    public string Title {
      get; private set;
    }


    public string Type {
      get; private set;
    }


    public string Column {
      get; private set;
    }


    public int Digits {
      get; private set;
    }


    public bool Show {
      get {
        return Column.Length != 0 && !Hidden;
      }
    }

    private bool Hidden {
      get; set;
    }

    #endregion Properties

  }  // class DataTableColumn

}  // namespace Empiria.FinancialAccounting
