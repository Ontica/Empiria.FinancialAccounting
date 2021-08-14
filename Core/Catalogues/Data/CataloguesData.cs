/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Calendar Management                        Component : Data Access Layer                       *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Service                            *
*  Type     : CataloguesData                             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data access layer for accounting system catalogues.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.Data {

  /// <summary>Data access layer for accounting system catalogues.</summary>
  static internal class CataloguesData {

    static internal FixedList<EventType> EventTypes() {
      var sql = "SELECT * " +
                "FROM COF_EVENTOS_CARTERA " +
                "ORDER BY COF_EVENTOS_CARTERA.DESCRICPION_EVENTO";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<EventType>(dataOperation);
    }

  }  // class CataloguesData

}  // namespace Empiria.FinancialAccounting.Data
