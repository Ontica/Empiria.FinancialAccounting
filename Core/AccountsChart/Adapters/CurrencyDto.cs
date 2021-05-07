/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Transfer Object                    *
*  Type     : CurrencyDto                                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Currency data transfer object.                                                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Currency data transfer object.</summary>
  public class CurrencyDto {

    internal CurrencyDto() {
      // no-op
    }


    public string UID {
      get; internal set;
    }


    public string Name {
      get; internal set;
    }


    public string Code {
      get; internal set;
    }


    public string Abbrev {
      get; internal set;
    }


    public string Symbol {
      get; internal set;
    }

  }  // class CurrencyDto

}  // namespace Empiria.FinancialAccounting.Adapters
