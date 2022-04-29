/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Catalogues Management                      Component : Data Access Layer                       *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Service                            *
*  Type     : ParticipantData                            License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data access layer for participants.                                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.Data {

  /// <summary>Data access layer for participants.</summary>
  static internal class ParticipantData {

    static internal FixedList<Participant> SearchParticipants(string filter) {
      var sql = "SELECT * FROM VW_MH_PARTICIPANTS " +
          $"WHERE {filter}" +
          $"ORDER BY PARTICIPANTNAME";

      var dataOperation = DataOperation.Parse(sql);

      return DataReader.GetFixedList<Participant>(dataOperation);
    }

  }  // class ParticipantData

}  // namespace Empiria.FinancialAccounting.Data
