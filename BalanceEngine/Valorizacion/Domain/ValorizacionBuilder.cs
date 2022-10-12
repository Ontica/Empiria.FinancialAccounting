/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : ValorizacionBuilder                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de valorizacion.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte de valorizacion.</summary>
  internal class ValorizacionBuilder {

    private readonly TrialBalanceQuery _query;

    public ValorizacionBuilder(TrialBalanceQuery query) {

      _query = query;

    }


    #region Public methods


    internal FixedList<ValorizacionEntry> Build() {

      throw new NotImplementedException();
       
    }


    #endregion Public methods


  } // class ValorizacionBuilder

} // namespace Empiria.FinancialAccounting.BalanceEngine
