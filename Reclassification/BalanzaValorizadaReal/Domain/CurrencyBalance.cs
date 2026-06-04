/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification Services                  Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reclassification.dll   Pattern   : Information Holder                      *
*  Type     : CurrencyBalance                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds trial accounting balances for a given currency.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.Reclassification {

  /// <summary>Holds trial accounting balances for a given currency.</summary>
  public class CurrencyBalance {

    #region Constructors and Parsers

    internal CurrencyBalance() {
      // Required by Empiria Framework
    }

    #endregion Constructors and Parsers

    #region Properties

    public Currency Currency {
      get; internal set;
    }

    public decimal InitialBalance {
      get; internal set;
    }

    public decimal Debits {
      get; internal set;
    }

    public decimal Credits {
      get; internal set;
    }

    public decimal FinalBalance {
      get; internal set;
    }

    #endregion Properties

  } // class CurrencyBalance

} // namespace Empiria.FinancialAccounting.Reclassification
