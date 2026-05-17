/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification Services                  Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reclassification.dll   Pattern   : Information Holder                      *
*  Type     : CurrencyBalance                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Used to read valorize balance by currency.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting.Reclassification {

  /// <summary>sed to read valorize balance by currency.</summary>
  public class CurrencyBalance {

    #region Constructors and Parsers

    internal CurrencyBalance() {
      // Required by Empiria Framework
    }

    #endregion Constructors and Parsers

    #region Properties

    public Currency Currency {
      get; set;
    }

    public decimal InitialBalance {
      get; set;
    }

    public decimal Debits {
      get; set;
    }

    public decimal Credits {
      get; set;
    }

    public decimal FinalBalance {
      get; set;
    }


    #endregion Properties

  } // class CurrencyBalance

} // namespace Empiria.FinancialAccounting.Reclassification
