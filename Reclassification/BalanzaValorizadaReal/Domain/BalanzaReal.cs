/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification Services                  Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reclassification.dll   Pattern   : Information Holder                      *
*  Type     : BalanzaReal                                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Used to read Real valorize balance.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Collections.Generic;

namespace Empiria.FinancialAccounting.Reclassification {

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

} // namespace Empiria.FinancialAccounting.Reclassification
