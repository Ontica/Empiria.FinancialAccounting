/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Data Layer                              *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Service                            *
*  Type     : BalancesDataService                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds clauses used in balances data service.                                                   *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.Data;


namespace Empiria.FinancialAccounting.BalanceEngine.Data {

  /// <summary>Holds clauses used in balances data service.</summary>
  static internal class TrialBalanceValuedDataService {


    #region Public methods

    static internal FixedList<TrialBalanceValued> GetTransactions(DateTime fromDate, DateTime toDate, int ExchangeRateTypeId) {

      var sql = "select " +
                "fecha_afectacion, id_cuenta_estandar, naturaleza, id_cuenta_auxiliar, " +
                "id_sector, id_moneda, debe, haber, exchange_rate_type_id, exchange_rate, " +
                "round(debe * exchange_rate, 4) debe_val, " +
                "round(haber * exchange_rate, 4) haber_val " +
                "from( " +
                "select " +
                "fecha_afectacion, id_cuenta_estandar, naturaleza, id_cuenta_auxiliar, " +
                "id_sector, id_moneda, sum(debe) Debe, sum(haber) Haber " +
                "from vw_cof_movimiento_bis " +
               $"where fecha_afectacion >= to_date('{fromDate.ToShortDateString()}', 'DD-MM-YYYY') " +
               $"and fecha_afectacion <= to_date('{toDate.ToShortDateString()}', 'DD-MM-YYYY') " +
                "group by fecha_afectacion, id_cuenta_estandar, naturaleza, id_cuenta_auxiliar, " +
                "id_sector, id_moneda " +
                ") movtos " +
               $"inner join(select * from ao_exchange_rates where exchange_rate_type_id = {ExchangeRateTypeId}) tc " +
                "on movtos.id_moneda = tc.to_currency_id " +
                "and movtos.fecha_afectacion = tc.from_date " +
                "order by fecha_afectacion, id_cuenta_estandar, naturaleza, id_cuenta_auxiliar,  " +
                "id_sector, id_moneda ";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<TrialBalanceValued>(op);
    }

    #endregion Public methods

  } // class BalanceDataServiceClauses

} // namespace Empiria.FinancialAccounting.BalanceEngine.Data
