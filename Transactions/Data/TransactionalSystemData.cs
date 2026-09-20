/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Transactions Management                        Component : Data Access Layer                   *
*  Assembly : FinancialAccounting.Transactions.dll           Pattern   : Data Service                        *
*  Type     : TransactionalSystemData                        License   : Please read LICENSE.txt file        *
*                                                                                                            *
*  Summary  : Data services for transactional systems.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using Empiria.Data;

namespace Empiria.FinancialAccounting.Transactions.Data {

  /// <summary>Data services for transactional systems.</summary>
  static internal class TransactionalSystemData {

    static internal FixedList<TransactionalSystemRule> GetTransactionalSystemRules(TransactionalSystem system) {
      var sql = "SELECT * " +
                "FROM COF_MAPEO_SISTEMAS_POLIZAS " +
               $"WHERE ID_SISTEMA = {system.Id} ";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<TransactionalSystemRule>(op);
    }

  }  // class TransactionalSystemData

}  // namespace Empiria.FinancialAccounting.Transactions.Data
