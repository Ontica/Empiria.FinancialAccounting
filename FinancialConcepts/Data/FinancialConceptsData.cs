/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Data Access Layer                       *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Data Service                            *
*  Type     : FinancialConceptsData                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data access layer for financial concepts.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.Data;

namespace Empiria.FinancialAccounting.FinancialConcepts.Data {

  /// <summary>Data access layer for financial concepts.</summary>
  static internal class FinancialConceptsData {

    static internal List<FinancialConcept> GetFinancialConcepts(FinancialConceptGroup group) {
      var sql = "SELECT * FROM COF_CONCEPTOS " +
                $"WHERE ID_GRUPO = {group.Id} " +
                "ORDER BY POSICION";

      var op = DataOperation.Parse(sql);

      return DataReader.GetList<FinancialConcept>(op);
    }


    internal static FixedList<FinancialConceptEntry> GetFinancialConceptEntries(FinancialConcept concept) {
      var sql = "SELECT COF_CONCEPTOS_INTEGRACION.* " +
                "FROM COF_CONCEPTOS_INTEGRACION " +
               $"WHERE ID_CONCEPTO = {concept.Id} " +
                "ORDER BY POSICION";

      var op = DataOperation.Parse(sql);

      return DataReader.GetFixedList<FinancialConceptEntry>(op);
    }

    static internal FixedList<FinancialConceptEntry> GetAllIntegrationEntriesForAGroup(FinancialConceptGroup group) {
      var sql = "SELECT COF_CONCEPTOS_INTEGRACION.* " +
                "FROM COF_CONCEPTOS_INTEGRACION " +
               $"WHERE ID_GRUPO = {group.Id} " +
                "ORDER BY POSICION";

      var op = DataOperation.Parse(sql);

      return DataReader.GetFixedList<FinancialConceptEntry>(op);
    }


    static internal void Write(FinancialConcept o) {
      var op = DataOperation.Parse("write_cof_concepto",
                                   o.Id, o.Code, o.Name, o.Position,
                                   o.StartDate, o.EndDate, o.UID,
                                   o.Group.Id);


      DataWriter.Execute(op);
    }


    static internal void Write(FinancialConceptEntry o) {
      var op = DataOperation.Parse("write_cof_concepto_integracion",
                      o.Id, o.UID, o.FinancialConcept.Id, o.CalculationRule, o.ReferencedFinancialConcept.Id,
                      o.AccountNumber, o.SubledgerAccountNumber, o.SectorCode, o.ExternalVariableCode,
                      (char) o.Operator, o.Qualification, o.IntegrationTypeId, o.Position,
                      o.Group.Id, o.CurrencyCode, o.AccountsListId);

      DataWriter.Execute(op);
    }


  }  // class FinancialConceptsData

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Data
