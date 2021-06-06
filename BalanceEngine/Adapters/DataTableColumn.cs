/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Transfer Object                    *
*  Type     : DataTableColumn                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return trial balances.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  public class DataTableColumn {

    public DataTableColumn(string field, string title, string type) {
      this.Field = field;
      this.Title = title;
      this.Type = type;
    }


    public string Field {
      get; private set;
    }


    public string Title {
      get; private set;
    }


    public string Type {
      get; private set;
    }

  }  // class DataTableColumn

}
