/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Banobras Integration Services                Component : Domain Layer                          *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll  Pattern   : Information Holder                    *
*  Type     : CalificacionMoneda                           License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Data holder for Califica_Moneda data table.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Collections;
using Empiria.FinancialAccounting.BanobrasIntegration.Data;

namespace Empiria.FinancialAccounting.BanobrasIntegration {

  internal class CalificacionMoneda {

    static private EmpiriaHashTable<CalificacionMoneda> _cache;

    static CalificacionMoneda() {

    }
    protected CalificacionMoneda() {
      // Required by Empiria Framework
    }

    static internal FixedList<CalificacionMoneda> GetList() {
      EnsureCacheIsLoaded();

      return _cache.ToFixedList();
    }

    static internal CalificacionMoneda TryParse(string cuenta, string sector, string auxiliar) {
      EnsureCacheIsLoaded();

      string hash = BuildHash(cuenta, sector, auxiliar);

      if (_cache.ContainsKey(hash)) {
        return _cache[hash];
      } else {
        return null;
      }
    }

    [DataField("CUENTA")]
    internal string Cuenta {
      get; private set;
    }


    [DataField("SECTOR")]
    internal string Sector {
      get; private set;
    }


    [DataField("AUXILIAR")]
    public string Auxiliar {
      get; private set;
    }


    [DataField("CALIFICA_SALDO", ConvertFrom=typeof(long))]
    public int CalificaSaldo {
      get; private set;
    }

    private static string BuildHash(string cuenta, string sector, string auxiliar) {
      return $"{cuenta}||{sector}||{auxiliar}";
    }

    static private void EnsureCacheIsLoaded() {
      if (_cache == null) {
        _cache = ExportBalancesDataService.GetCalificacionMonedaHashTable();
      }
    }


  }  // class CalificacionMoneda

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration
