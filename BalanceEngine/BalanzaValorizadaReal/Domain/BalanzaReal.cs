/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Information Holder                      *
*  Type     : BalanzaValorizadaReal                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Used to read Real valorize balance.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Collections.Generic;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Used to read Real valorize balance.</summary>
  public class BalanzaReal {

    #region Constructors and Parsers

    internal BalanzaReal() {
      // Required by Empiria Framework
    }

    #endregion Constructors and Parsers

    #region Properties

    public StandardAccount CuentaEstandar {
      get; set;
    }

    public List<CurrencyBalance> SaldosPorMoneda {
      get; set;
    } = new List<CurrencyBalance>();

    public List<CurrencyBalance> SaldosPorMonedaReal {
      get; set;
    } = new List<CurrencyBalance>();


    #endregion Properties

  } // class BalanzaValorizadaReal

} // namespace Empiria.FinancialAccounting.BalanceEngine
