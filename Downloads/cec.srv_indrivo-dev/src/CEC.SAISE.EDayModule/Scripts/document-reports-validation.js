const validationMsgs = {
    isRequired: 'Câmp obligatoriu',
    max3500: 'maximum 3500',
    max15: 'maximum 15',
    max10: 'maximum 10',
    max5: 'maximum 5',
    digitsExpected: 'Se așteaptă doar cifre',
    C_GT_4A_B_Rule_Failed: 'C trebuie sa fie mai mare sau egal cu 4A + B',
    A_eq_B_C_Rule_Failed: 'A trebuie sa fie egal cu B + C',
    C_GT_A_B_Rule_Failed: 'Numărul de alegători incluşi în listele electorale <span class="badge">a</span> şi în listele suplimentare <span class="badge">b</span> este mai mic decît numărul de alegători care au primit buletine de vot <span class="badge">c</span>.',
    C_LT_D_Rule_Failed: 'Numărul de alegători care au participat la votare <span class="badge">d</span> este mai mare decît numărul de alegători care au primit buletine de vot <span class="badge">c</span>.',
    E_NE_C_D_Rule_Failed: 'Сifra ce reflectă diferenţa dintre numărul buletinelor de vot primite de alegători şi numărul alegătorilor care au participat la votare <span class="badge">e</span> este indicată incorect.',
    H_eq_Sum_of_g_Rule_Failed: 'Numărul total de voturi valabil exprimate <span class="badge">h</span> trebuie să fie egal cu suma voturilor tuturor concurenților electorali <span class="badge">g1</span>...<span class="badge">gn</span>.',
    D_eq_F_H_Rule_Failed: 'Numărul de alegători care au participat la votare <span class="badge">d</span> trebuie să fie egal cu numărul de buletine nevalabile <span class="badge">f</span> plus numărul de buletine valabile <span class="badge">h</span>.',
    I_eq_C_J_Rule: 'Numărul de buletine de vot primite de biroul electoral al secției de votare <span class="badge">i</span> trebuie să fie egal cu numărul de alegători care au primit buletine de vot <span class="badge">c</span> şi numărul de buletine neutilizate şi anulate <span class="badge">j</span>.',
    J_eq_I_diff_C_Rule: 'La numărul buletinelor de vot neutilizate și anulate <span class="badge">j</span> nu se includ buletinele de vot declarate nevalabile <span class="badge">f</span>***',
    D_eq_or_GT_4A_4B_C_Rule_Failed: 'D trebuie sa fie mai mare sau egal cu 4A + 4B + C',
    A_eq_9A_9E_9F_9G_Rule_Failed: 'A trebuie sa fie egal cu 9(a) + 9(e) + 9(f) + 9(g)',
    A_eq_B_C_D_Rule_Failed: 'A trebuie sa fie egal cu b + c + d',
    A_B_C_eq_D_E_F_eq_G_H_Rule_Failed: 'a + b + c trebuie sa fie egal cu d + e + f și cu g + h',
    D_equal_or_less_than_C_Rule_Failed: 'd trebuie sa fie egal sau mai mic decât c',
    A_equal_point5_minus_point21_Rule_Failed: 'a trebuie sa fie egal cu cifra de la pct. 5 – pct. 21',
    equal_or_less_than_point_6_Rule_Failed: '21 trebuie sa fie egal sa mai mic decât cifra de la pct. 6',
    A_eq_5A_5E_5F_5G_Rule_Failed: '',
    A_eq_6A_6E_6F_6G_Rule_Failed: '',
    A_eq_B_C_D_CECE_1_day_1st_round_Rule_Before_Failed: '',
    A_eq_B_C_D_CECE_1_day_1st_round_Rule_Failed: '',
    qual_or_less_than_point_1_Rule_Failed: '',


    isRequiredField: function (x) {
        return '<span class="badge">' + x + '</span> este valoare obligatorie.';
    },
    filedMustBe_GT_Zero: function (x) {
        return '<span class="badge">' + x + '</span> trebuie să fie mai mare ca 0';
    },
    gt_Zero_IfLocal: function (x, isLocal) {
        if (isLocal) {
            return '<span class="badge">' + x + '</span> trebuie să fie mai mare ca 0';
        } else {
            return '<span class="badge">' + x + '</span> trebuie să fie mai mare sau egal ca 0';
        }
    }
};

const validationRules = {
    notGreaterThan10: function (a, params) {
        const result = parseInt(a) <= 10;

        StationaryVotingBoxes80L_Rule_Succeeded(result);

        return result;
    },

    C_GT_4A_B_Rule: function (c, params) {
        if (params.bp.StationaryBalltoBoxesCount == undefined || params.bp.MobileBallotBoxesCount == undefined) {
            return false;
        }

        const result = parseInt(c) >= (4 * parseInt(params.bp.StationaryBalltoBoxesCount()) + parseInt(params.bp.MobileBallotBoxesCount()));
        params.bp.PlasticSealsCount_Rule_Succeeded(result);
        return result;
    },

    A_eq_B_D_Rule: function (a, params) {
        if (params.bp.CertsHandedToVoters == undefined || params.bp.CertsUnusedCancelled == undefined) {
            return false;
        }

        var result = parseInt(a) === (parseInt(params.bp.CertsHandedToVoters()) + parseInt(params.bp.CertsUnusedCancelled()));

        params.bp.CertsFromCECECount_Rule_Succeeded(result);

        return result;
    },

    A_eq_B_D_Final_BESV_2nd_round_Rule: function (f, params) {
        if (params.bp.VotersIssuedCertificates == undefined || params.bp.UnusedOrCancelledCertificates == undefined) {
            return false;
        }

        var result = parseInt(f) === (parseInt(params.bp.VotersIssuedCertificates()) + parseInt(params.bp.UnusedOrCancelledCertificates()));

        params.bp.CECEReceivedCertificates_Rule_Succeeded(result);

        return result;
    },

    A_eq_B_D_BESV_Final_Rule: function (f, params) {
        if (params.bp.VotersIssuedCertificatesCount == undefined || params.bp.UnusedOrCancelledCertificatesCount == undefined) {
            return false;
        }

        var result = parseInt(f) === (parseInt(params.bp.VotersIssuedCertificatesCount()) + parseInt(params.bp.UnusedOrCancelledCertificatesCount()));

        params.bp.CECEReceivedCertificatesCount_Rule_Succeeded(result);

        return result;
    },

    A_eq_B_D_BESV_Final_2days_Rule: function (f, params) {
        if (params.bp.CertificatesIssuedToVoters == undefined || params.bp.UnusedOrCancelledCertificates == undefined) {
            return false;
        }

        var result = parseInt(f) === (parseInt(params.bp.CertificatesIssuedToVoters()) + parseInt(params.bp.UnusedOrCancelledCertificates()));

        params.bp.CertificatesReceivedFromCECE_Rule_Succeeded(result);

        return result;
    },

    A_eq_B_D_Final_BESV_Referendum_2days_Rule: function (f, params) {
        if (params.bp.CertificatesIssuedToVoters == undefined || params.bp.UnusedOrCancelledCertificates == undefined) {
            return false;
        }

        var result = parseInt(f) === (parseInt(params.bp.CertificatesIssuedToVoters()) + parseInt(params.bp.UnusedOrCancelledCertificates()));

        params.bp.CertificatesReceivedFromCECE_Rule_Succeeded(result);

        return result;
    },

    A_eq_B_D_Final_BESV_Referendum_Rule: function (f, params) {
        if (params.bp.CertificatesIssuedToVoters == undefined || params.bp.UnusedOrCancelledCertificates == undefined) {
            return false;
        }

        var result = parseInt(f) === (parseInt(params.bp.CertificatesIssuedToVoters()) + parseInt(params.bp.UnusedOrCancelledCertificates()));

        params.bp.CertificatesReceivedFromCECE_Rule_Succeeded(result);

        return result;
    },

    C_GT_4A_B_Rule_Second_Day: function (c, params) {
        if (params.bp.StationaryBalltoBoxesCount == undefined || params.bp.MobileBallotBoxesCount == undefined) {
            return false;
        }

        const result = parseInt(c) >= (4 * parseInt(params.bp.StationaryBalltoBoxesCount()) + parseInt(params.bp.MobileBallotBoxesCount()));
        params.bp.PlasticSealsUsedCount_Rule_Succeeded(result);
        return result;
    },

    D_eq_or_GT_4A_4B_C_Rule: function (d, params) {
        if (params.bp.StationaryVotingBoxes80L == undefined || params.bp.StationaryVotingBoxes45L == undefined || params.bp.MobileVotingBoxes27L == undefined) {
            return false;
        }

        const result = parseInt(d) >= 4 * parseInt(params.bp.StationaryVotingBoxes80L()) +
            4 * parseInt(params.bp.StationaryVotingBoxes45L()) +
            parseInt(params.bp.MobileVotingBoxes27L());

        params.bp.PlasticSealsUsedForVotingBoxesCount_Rule_Succeeded(result);

        return result;
    },

    D_eq_or_GT_4A_4B_C_Rule_Second_Round: function (d, params) {
        if (params.bp.StationaryVotingBox80Liters == undefined || params.bp.StationaryVotingBox45Liters == undefined || params.bp.MobileVotingBox27Liters == undefined) {
            return false;
        }

        const result = parseInt(d) >= 4 * parseInt(params.bp.StationaryVotingBox80Liters()) +
            4 * parseInt(params.bp.StationaryVotingBox45Liters()) +
            parseInt(params.bp.MobileVotingBox27Liters());

        params.bp.PlasticSealsForVotingBoxCount_Rule_Succeeded(result);

        return result;
    },

    D_eq_or_GT_4A_4B_C_2days_Rule: function (d, params) {
        if (params.bp.StationaryVotingBox80Liters == undefined || params.bp.StationaryVotingBox45Liters == undefined || params.bp.MobileVotingBox27Liters == undefined) {
            return false;
        }

        const result = parseInt(d) >= 4 * parseInt(params.bp.StationaryVotingBox80Liters()) +
            4 * parseInt(params.bp.StationaryVotingBox45Liters()) +
            parseInt(params.bp.MobileVotingBox27Liters());

        params.bp.PlasticSealsForVotingBoxCount_Rule_Succeeded(result);

        return result;
    },

    A_eq_9A_9E_9F_9G_Rule: function (a, params) {
        if (
            params.bp.TotalDecisionsOnObjectionsCountBefore == undefined ||
            params.bp.WrittenResponsesToObjectionsCountBefore == undefined ||
            params.bp.ObjectionsForwardedToOtherAuthoritiesCountBefore == undefined ||
            params.bp.ReturnedObjectionsCountBefore == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.TotalDecisionsOnObjectionsCountBefore()) +
            parseInt(params.bp.WrittenResponsesToObjectionsCountBefore()) +
            parseInt(params.bp.ObjectionsForwardedToOtherAuthoritiesCountBefore()) +
            parseInt(params.bp.ReturnedObjectionsCountBefore());

        params.bp.DepositedBeforeVotingDayCount_Rule_Succeeded(result);

        return result;
    },

    A_eq_9A_9E_9F_9G_Final_BESV_Rule: function (a, params) {
        if (
            params.bp.TotalDecisionsOnObjectionsCountBefore == undefined ||
            params.bp.WrittenResponsesToObjectionsCountBefore == undefined ||
            params.bp.ObjectionsForwardedToOtherAuthoritiesCountBefore == undefined ||
            params.bp.ReturnedObjectionsCountBefore == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.TotalDecisionsOnObjectionsCountBefore()) +
            parseInt(params.bp.WrittenResponsesToObjectionsCountBefore()) +
            parseInt(params.bp.ObjectionsForwardedToOtherAuthoritiesCountBefore()) +
            parseInt(params.bp.ReturnedObjectionsCountBefore());

        params.bp.ContestationsUntilEDay_Rule_Succeeded(result);

        return result;
    },

    A_eq_9A_9E_9F_9G_Final_BESV_Referendum_1day_Rule: function (a, params) {
        if (
            params.bp.TotalDecisionsOnObjectionsBefore == undefined ||
            params.bp.ObjectionResponsesByLetterBefore == undefined ||
            params.bp.ObjectionsForwardedToOtherAuthoritiesBefore == undefined ||
            params.bp.ReturnedObjectionsBefore == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.TotalDecisionsOnObjectionsBefore()) +
            parseInt(params.bp.ObjectionResponsesByLetterBefore()) +
            parseInt(params.bp.ObjectionsForwardedToOtherAuthoritiesBefore()) +
            parseInt(params.bp.ReturnedObjectionsBefore());

        params.bp.ContestationsUntilEDay_Rule_Succeeded(result);

        return result;
    },

    A_eq_9A_9E_9F_9G_Final_BESV_2nd_round_Rule: function (a, params) {
        if (
            params.bp.TotalDecisionsOnObjectionsBefore == undefined ||
            params.bp.ObjectionResponsesByLetterBefore == undefined ||
            params.bp.ObjectionsForwardedToOtherAuthoritiesBefore == undefined ||
            params.bp.ReturnedObjectionsBefore == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.TotalDecisionsOnObjectionsBefore()) +
            parseInt(params.bp.ObjectionResponsesByLetterBefore()) +
            parseInt(params.bp.ObjectionsForwardedToOtherAuthoritiesBefore()) +
            parseInt(params.bp.ReturnedObjectionsBefore());

        params.bp.ContestationsUntilEDay_Rule_Succeeded(result);

        return result;
    },

    A_eq_9A_9E_9F_9G_Final_BESV_elections_2nd_round_Rule: function (a, params) {
        if (
            params.bp.TotalDecisionsOnObjectionsBefore == undefined ||
            params.bp.ObjectionResponsesByLetterBefore == undefined ||
            params.bp.ObjectionsForwardedToOtherAuthoritiesBefore == undefined ||
            params.bp.ReturnedObjectionsBefore == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.TotalDecisionsOnObjectionsBefore()) +
            parseInt(params.bp.ObjectionResponsesByLetterBefore()) +
            parseInt(params.bp.ObjectionsForwardedToOtherAuthoritiesBefore()) +
            parseInt(params.bp.ReturnedObjectionsBefore());

        params.bp.ContestationsUntilEDay_Rule_Succeeded(result);

        return result;
    },

    A_eq_10A_10E_10F_10G_Rule: function (a, params) {
        if (
            params.bp.TotalDecisionsOnObjectionsCount == undefined ||
            params.bp.WrittenResponsesToObjectionsCount == undefined ||
            params.bp.ObjectionsForwardedToOtherAuthoritiesCount == undefined ||
            params.bp.ReturnedObjectionsCount == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.TotalDecisionsOnObjectionsCount()) +
            parseInt(params.bp.WrittenResponsesToObjectionsCount()) +
            parseInt(params.bp.ObjectionsForwardedToOtherAuthoritiesCount()) +
            parseInt(params.bp.ReturnedObjectionsCount());

        params.bp.DepositedOnVotingDayCount_Rule_Succeeded(result);

        return result;
    },

    A_eq_10A_10E_10F_10G_Final_BESV_Rule: function (a, params) {
        if (
            params.bp.TotalDecisionsOnObjectionsCount == undefined ||
            params.bp.WrittenResponsesToObjectionsCount == undefined ||
            params.bp.ObjectionsForwardedToOtherAuthoritiesCount == undefined ||
            params.bp.ReturnedObjectionsCount == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.TotalDecisionsOnObjectionsCount()) +
            parseInt(params.bp.WrittenResponsesToObjectionsCount()) +
            parseInt(params.bp.ObjectionsForwardedToOtherAuthoritiesCount()) +
            parseInt(params.bp.ReturnedObjectionsCount());

        params.bp.ContestationsOnEDay_Rule_Succeeded(result);

        return result;
    },

    A_eq_10A_10E_10F_10G_2nd_round_Rule: function (a, params) {
        if (
            params.bp.TotalDecisionsOnObjectionsCount == undefined ||
            params.bp.WrittenResponsesToObjectionsCount == undefined ||
            params.bp.ObjectionsForwardedToOtherAuthoritiesCount == undefined ||
            params.bp.ReturnedObjectionsCount == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.TotalDecisionsOnObjectionsCount()) +
            parseInt(params.bp.WrittenResponsesToObjectionsCount()) +
            parseInt(params.bp.ObjectionsForwardedToOtherAuthoritiesCount()) +
            parseInt(params.bp.ReturnedObjectionsCount());

        params.bp.SubmittedOnVotingDayCount_Rule_Succeeded(result);

        return result;
    },

    A_eq_10A_10E_10F_10G_Final_BESV_2nd_round_Rule: function (a, params) {
        if (
            params.bp.TotalDecisionsOnObjectionsCount == undefined ||
            params.bp.WrittenResponsesToObjectionsCount == undefined ||
            params.bp.ObjectionsForwardedToOtherAuthoritiesCount == undefined ||
            params.bp.ReturnedObjectionsCount == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.TotalDecisionsOnObjectionsCount()) +
            parseInt(params.bp.WrittenResponsesToObjectionsCount()) +
            parseInt(params.bp.ObjectionsForwardedToOtherAuthoritiesCount()) +
            parseInt(params.bp.ReturnedObjectionsCount());

        params.bp.ContestationsOnEDay_Rule_Succeeded(result);

        return result;
    },

    A_eq_10A_10E_10F_10G_2days_Rule: function (a, params) {
        if (
            params.bp.TotalDecisionsOnObjectionsBefore == undefined ||
            params.bp.ObjectionResponsesByLetterBefore == undefined ||
            params.bp.ObjectionsForwardedToOtherAuthoritiesBefore == undefined ||
            params.bp.ReturnedObjectionsBefore == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.TotalDecisionsOnObjectionsBefore()) +
            parseInt(params.bp.ObjectionResponsesByLetterBefore()) +
            parseInt(params.bp.ObjectionsForwardedToOtherAuthoritiesBefore()) +
            parseInt(params.bp.ReturnedObjectionsBefore());

        params.bp.ContestationsUntilEDay_Rule_Succeeded(result);

        return result;
    },

    A_eq_11A_11E_11F_11G_2days_Rule: function (a, params) {
        if (
            params.bp.TotalDecisionsOnObjectionsFirstDay == undefined ||
            params.bp.ObjectionResponsesByLetterFirstDay == undefined ||
            params.bp.ObjectionsForwardedToOtherBodiesFirstDay == undefined ||
            params.bp.ReturnedObjectionsFirstDay == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.TotalDecisionsOnObjectionsFirstDay()) +
            parseInt(params.bp.ObjectionResponsesByLetterFirstDay()) +
            parseInt(params.bp.ObjectionsForwardedToOtherBodiesFirstDay()) +
            parseInt(params.bp.ReturnedObjectionsFirstDay());

        params.bp.ContestationsOnEDay_Rule_Succeeded(result);

        return result;
    },

    A_eq_10A_10E_10F_10G_Final_BESV_Referendum_1day_Rule: function (a, params) {
        if (
            params.bp.TotalDecisionsOnObjectionsFirstDay == undefined ||
            params.bp.ObjectionResponsesByLetterFirstDay == undefined ||
            params.bp.ObjectionsForwardedToOtherAuthoritiesFirstDay == undefined ||
            params.bp.ReturnedObjectionsFirstDay == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.TotalDecisionsOnObjectionsFirstDay()) +
            parseInt(params.bp.ObjectionResponsesByLetterFirstDay()) +
            parseInt(params.bp.ObjectionsForwardedToOtherAuthoritiesFirstDay()) +
            parseInt(params.bp.ReturnedObjectionsFirstDay());

        params.bp.ContestationsOnEDay_Rule_Succeeded(result);

        return result;
    },

    A_eq_12A_12E_12F_12G_2days_Rule: function (a, params) {
        if (
            params.bp.TotalDecisionsOnObjectionsSecondDay == undefined ||
            params.bp.ObjectionResponsesByLetterSecondDay == undefined ||
            params.bp.ObjectionsForwardedToOtherBodiesSecondDay == undefined ||
            params.bp.ReturnedObjectionsSecondDay == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.TotalDecisionsOnObjectionsSecondDay()) +
            parseInt(params.bp.ObjectionResponsesByLetterSecondDay()) +
            parseInt(params.bp.ObjectionsForwardedToOtherBodiesSecondDay()) +
            parseInt(params.bp.ReturnedObjectionsSecondDay());

        params.bp.SubmittedOnVotingDayCountSecondDay_Rule_Succeeded(result);

        return result;
    },

    A_eq_5A_5E_5F_5G_Rule: function (a, params) {
        if (
            params.bp.TotalDecisionsOnContestationsUntilEDay == undefined ||
            params.bp.ResponsesToContestationsByLetter == undefined ||
            params.bp.ContestationsRemittedToOtherAuthorities == undefined ||
            params.bp.ContestationsReturned == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.TotalDecisionsOnContestationsUntilEDay()) +
            parseInt(params.bp.ResponsesToContestationsByLetter()) +
            parseInt(params.bp.ContestationsRemittedToOtherAuthorities()) +
            parseInt(params.bp.ContestationsReturned());

        params.bp.ContestationsUntilEDayCount_Rule_Succeeded(result);

        return result;
    },

    A_eq_5A_5E_5F_5G_CECE_Rule: function (a, params) {
        if (
            params.bp.TotalDecisionsOnContestationsUntilEDay == undefined ||
            params.bp.ResponsesToContestationsByLetter == undefined ||
            params.bp.ContestationsRemittedToOtherAuthorities == undefined ||
            params.bp.ContestationsReturned == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.TotalDecisionsOnContestationsUntilEDay()) +
            parseInt(params.bp.ResponsesToContestationsByLetter()) +
            parseInt(params.bp.ContestationsRemittedToOtherAuthorities()) +
            parseInt(params.bp.ContestationsReturned());

        params.bp.ContestationsSubmittedToCouncilBefore_Rule_Succeeded(result);

        return result;
    },

    A_eq_5A_5E_5F_5G_CECE_referendum_Rule: function (a, params) {
        if (
            params.bp.TotalDecisionsOnContestationsUntilEDay == undefined ||
            params.bp.ResponsesToContestationsByLetter == undefined ||
            params.bp.ContestationsRemittedToOtherAuthorities == undefined ||
            params.bp.ContestationsReturned == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.TotalDecisionsOnContestationsUntilEDay()) +
            parseInt(params.bp.ResponsesToContestationsByLetter()) +
            parseInt(params.bp.ContestationsRemittedToOtherAuthorities()) +
            parseInt(params.bp.ContestationsReturned());

        params.bp.ContestationsUntilEDayCount_Rule_Succeeded(result);

        return result;
    },

    A_eq_5A_5E_5F_5G_CECE_2days_Rule: function (a, params) {
        if (
            params.bp.DecisionsOnContestationsMade == undefined ||
            params.bp.ResponsesToContestationsByLetter == undefined ||
            params.bp.ContestationsReferredToOtherAuthorities == undefined ||
            params.bp.ContestationsReturned == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.DecisionsOnContestationsMade()) +
            parseInt(params.bp.ResponsesToContestationsByLetter()) +
            parseInt(params.bp.ContestationsReferredToOtherAuthorities()) +
            parseInt(params.bp.ContestationsReturned());

        params.bp.ContestationsSubmittedToCouncil_Rule_Succeeded(result);

        return result;
    },

    A_eq_6A_6E_6F_6G_Rule: function (a, params) {
        if (
            params.bp.ContestationsOnEDayDecisions == undefined ||
            params.bp.ResponsesToContestationsOnEDayByLetter == undefined ||
            params.bp.ContestationsOnEDayRemittedToOtherAuthorities == undefined ||
            params.bp.ContestationsOnEDayReturned == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.ContestationsOnEDayDecisions()) +
            parseInt(params.bp.ResponsesToContestationsOnEDayByLetter()) +
            parseInt(params.bp.ContestationsOnEDayRemittedToOtherAuthorities()) +
            parseInt(params.bp.ContestationsOnEDayReturned());

        params.bp.ContestationsSubmittedToCouncilOnEDay_Rule_Succeeded(result);

        return result;
    },

    A_eq_6A_6E_6F_6G_Final_CECE_1st_round_Rule: function (a, params) {
        if (
            params.bp.ContestationsOnEDayDecisions == undefined ||
            params.bp.ResponsesToContestationsOnEDayByLetter == undefined ||
            params.bp.ContestationsOnEDayRemittedToOtherAuthorities == undefined ||
            params.bp.ContestationsOnEDayReturned == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.ContestationsOnEDayDecisions()) +
            parseInt(params.bp.ResponsesToContestationsOnEDayByLetter()) +
            parseInt(params.bp.ContestationsOnEDayRemittedToOtherAuthorities()) +
            parseInt(params.bp.ContestationsOnEDayReturned());

        params.bp.ContestationsOnEDayCount_Rule_Succeeded(result);

        return result;
    },

    A_eq_6A_6E_6F_6G_CECE_referendum_Rule: function (a, params) {
        if (
            params.bp.ContestationsOnEDayDecisions == undefined ||
            params.bp.ResponsesToContestationsOnEDayByLetter == undefined ||
            params.bp.ContestationsOnEDayRemittedToOtherAuthorities == undefined ||
            params.bp.ContestationsOnEDayReturned == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.ContestationsOnEDayDecisions()) +
            parseInt(params.bp.ResponsesToContestationsOnEDayByLetter()) +
            parseInt(params.bp.ContestationsOnEDayRemittedToOtherAuthorities()) +
            parseInt(params.bp.ContestationsOnEDayReturned());

        params.bp.ContestationsOnEDayCount_Rule_Succeeded(result);

        return result;
    },

    A_eq_6A_6E_6F_6G_CECE_2days_Rule: function (a, params) {
        if (
            params.bp.DecisionsOnContestationsMadeOnEDay == undefined ||
            params.bp.ResponsesToContestationsByLetterOnEDay == undefined ||
            params.bp.ContestationsReferredToOtherAuthoritiesOnEDay == undefined ||
            params.bp.ContestationsReturnedOnEDay == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.DecisionsOnContestationsMadeOnEDay()) +
            parseInt(params.bp.ResponsesToContestationsByLetterOnEDay()) +
            parseInt(params.bp.ContestationsReferredToOtherAuthoritiesOnEDay()) +
            parseInt(params.bp.ContestationsReturnedOnEDay());

        params.bp.ContestationsSubmittedToCouncilOnEDay_Rule_Succeeded(result);

        return result;
    },

    A_eq_7A_7E_7F_7G_CECE_2days_Rule: function (a, params) {
        if (
            params.bp.DecisionsOnContestationsMadeOnDay2OfVoting == undefined ||
            params.bp.ResponsesToContestationsByLetterOnDay2OfVoting == undefined ||
            params.bp.ContestationsReferredToOtherAuthoritiesOnDay2OfVoting == undefined ||
            params.bp.ContestationsReturnedOnDay2OfVoting == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.DecisionsOnContestationsMadeOnDay2OfVoting()) +
            parseInt(params.bp.ResponsesToContestationsByLetterOnDay2OfVoting()) +
            parseInt(params.bp.ContestationsReferredToOtherAuthoritiesOnDay2OfVoting()) +
            parseInt(params.bp.ContestationsReturnedOnDay2OfVoting());

        params.bp.ContestationsSubmittedToCouncilOnDay2OfVoting_Rule_Succeeded(result);

        return result;
    },

    A_eq_B_C_D_CECE_2days_Rule_Before: function (a, params) {
        if (
            params.bp.ContestationsAcceptedInFull == undefined ||
            params.bp.ContestationsAcceptedPartially == undefined ||
            params.bp.ContestationsRejected == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.ContestationsAcceptedInFull()) +
            parseInt(params.bp.ContestationsAcceptedPartially()) +
            parseInt(params.bp.ContestationsRejected());

        params.bp.DecisionsOnContestationsMade_Rule_Succeeded(result);

        return result;
    },

    A_eq_B_C_D_CECE_2days_Rule_1st_day: function (a, params) {
        if (
            params.bp.ContestationsAcceptedInFullOnEDay == undefined ||
            params.bp.ContestationsAcceptedPartiallyOnEDay == undefined ||
            params.bp.ContestationsRejectedOnEDay == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.ContestationsAcceptedInFullOnEDay()) +
            parseInt(params.bp.ContestationsAcceptedPartiallyOnEDay()) +
            parseInt(params.bp.ContestationsRejectedOnEDay());

        params.bp.DecisionsOnContestationsMadeOnEDay_Rule_Succeeded(result);

        return result;
    },

    A_eq_B_C_D_CECE_2days_Rule_2nd_day: function (a, params) {
        if (
            params.bp.ContestationsAcceptedInFullOnDay2OfVoting == undefined ||
            params.bp.ContestationsAcceptedPartiallyOnDay2OfVoting == undefined ||
            params.bp.ContestationsRejectedOnDay2OfVoting == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.ContestationsAcceptedInFullOnDay2OfVoting()) +
            parseInt(params.bp.ContestationsAcceptedPartiallyOnDay2OfVoting()) +
            parseInt(params.bp.ContestationsRejectedOnDay2OfVoting());

        params.bp.DecisionsOnContestationsMadeOnDay2OfVoting_Rule_Succeeded(result);

        return result;
    },

    A_eq_B_C_D_CECE_referendum_Rule_Before: function (a, params) {
        if (
            params.bp.DecisionsFullyAdmittingContestationsUntilEDay == undefined ||
            params.bp.DecisionsPartiallyAdmittingContestationsUntilEDay == undefined ||
            params.bp.ContestationsRejectedUntilEDay == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.DecisionsFullyAdmittingContestationsUntilEDay()) +
            parseInt(params.bp.DecisionsPartiallyAdmittingContestationsUntilEDay()) +
            parseInt(params.bp.ContestationsRejectedUntilEDay());

        params.bp.TotalDecisionsOnContestationsUntilEDay_Rule_Succeeded(result);

        return result;
    },

    A_eq_B_C_D_CECE_referendum_Rule: function (a, params) {
        if (
            params.bp.DecisionsFullyAdmittingContestationsOnEDay == undefined ||
            params.bp.DecisionsPartiallyAdmittingContestationsOnEDay == undefined ||
            params.bp.ContestationsRejectedOnEDay == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.DecisionsFullyAdmittingContestationsOnEDay()) +
            parseInt(params.bp.DecisionsPartiallyAdmittingContestationsOnEDay()) +
            parseInt(params.bp.ContestationsRejectedOnEDay());

        params.bp.ContestationsOnEDayDecisions_Rule_Succeeded(result);

        return result;
    },

    A_eq_B_C_D_Rule_Before: function (a, params) {
        if (
            params.bp.FullyAdmittedObjectionsCountBefore == undefined ||
            params.bp.PartiallyAdmittedObjectionsCountBefore == undefined ||
            params.bp.UnfoundedObjectionsCountBefore == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.FullyAdmittedObjectionsCountBefore()) +
            parseInt(params.bp.PartiallyAdmittedObjectionsCountBefore()) +
            parseInt(params.bp.UnfoundedObjectionsCountBefore());

        params.bp.TotalDecisionsOnObjectionsCountBefore_Rule_Succeeded(result);

        return result;
    },

    A_eq_B_C_D_2days_Rule_Before: function (a, params) {
        if (
            params.bp.DecisionsFullyAcceptedObjectionsBefore == undefined ||
            params.bp.DecisionsPartiallyAcceptedObjectionsBefore == undefined ||
            params.bp.DecisionsRejectedObjectionsBefore == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.DecisionsFullyAcceptedObjectionsBefore()) +
            parseInt(params.bp.DecisionsPartiallyAcceptedObjectionsBefore()) +
            parseInt(params.bp.DecisionsRejectedObjectionsBefore());

        params.bp.TotalDecisionsOnObjectionsBefore_Rule_Succeeded(result);

        return result;
    },

    A_eq_B_C_D_2days_1stDay_Rule_Before: function (a, params) {
        if (
            params.bp.DecisionsFullyAcceptedObjectionsFirstDay == undefined ||
            params.bp.DecisionsPartiallyAcceptedObjectionsFirstDay == undefined ||
            params.bp.DecisionsRejectedObjectionsFirstDay == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.DecisionsFullyAcceptedObjectionsFirstDay()) +
            parseInt(params.bp.DecisionsPartiallyAcceptedObjectionsFirstDay()) +
            parseInt(params.bp.DecisionsRejectedObjectionsFirstDay());

        params.bp.TotalDecisionsOnObjectionsFirstDay_Rule_Succeeded(result);

        return result;
    },

    A_eq_B_C_D_Final_BESV_Referendum_1day_Rule_Before: function (a, params) {
        if (
            params.bp.DecisionsFullyAcceptedObjectionsBefore == undefined ||
            params.bp.DecisionsPartiallyAcceptedObjectionsBefore == undefined ||
            params.bp.DecisionsRejectedObjectionsBefore == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.DecisionsFullyAcceptedObjectionsBefore()) +
            parseInt(params.bp.DecisionsPartiallyAcceptedObjectionsBefore()) +
            parseInt(params.bp.DecisionsRejectedObjectionsBefore());

        params.bp.TotalDecisionsOnObjectionsBefore_Rule_Succeeded(result);

        return result;
    },

    A_eq_B_C_D_Final_BESV_Referendum_1day_Rule_1stDay: function (a, params) {
        if (
            params.bp.DecisionsFullyAcceptedObjectionsFirstDay == undefined ||
            params.bp.DecisionsPartiallyAcceptedObjectionsFirstDay == undefined ||
            params.bp.DecisionsRejectedObjectionsFirstDay == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.DecisionsFullyAcceptedObjectionsFirstDay()) +
            parseInt(params.bp.DecisionsPartiallyAcceptedObjectionsFirstDay()) +
            parseInt(params.bp.DecisionsRejectedObjectionsFirstDay());

        params.bp.TotalDecisionsOnObjectionsFirstDay_Rule_Succeeded(result);

        return result;
    },

    A_eq_B_C_D_2days_2ndDay_Rule_Before: function (a, params) {
        if (
            params.bp.DecisionsFullyAcceptedObjectionsSecondDay == undefined ||
            params.bp.DecisionsPartiallyAcceptedObjectionsSecondDay == undefined ||
            params.bp.DecisionsRejectedObjectionsSecondDay == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.DecisionsFullyAcceptedObjectionsSecondDay()) +
            parseInt(params.bp.DecisionsPartiallyAcceptedObjectionsSecondDay()) +
            parseInt(params.bp.DecisionsRejectedObjectionsSecondDay());

        params.bp.TotalDecisionsOnObjectionsSecondDay_Rule_Succeeded(result);

        return result;
    },

    A_eq_B_C_D_Rule: function (a, params) {
        if (
            params.bp.FullyAdmittedObjectionsCount == undefined ||
            params.bp.PartiallyAdmittedObjectionsCount == undefined ||
            params.bp.UnfoundedObjectionsCount == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.FullyAdmittedObjectionsCount()) +
            parseInt(params.bp.PartiallyAdmittedObjectionsCount()) +
            parseInt(params.bp.UnfoundedObjectionsCount());

        params.bp.TotalDecisionsOnObjectionsCount_Rule_Succeeded(result);

        return result;
    },

    A_eq_B_C_D_2nd_round_before_Rule: function (a, params) {
        if (
            params.bp.DecisionsFullyAcceptedObjectionsBefore == undefined ||
            params.bp.DecisionsPartiallyAcceptedObjectionsBefore == undefined ||
            params.bp.DecisionsRejectedObjectionsBefore == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.DecisionsFullyAcceptedObjectionsBefore()) +
            parseInt(params.bp.DecisionsPartiallyAcceptedObjectionsBefore()) +
            parseInt(params.bp.DecisionsRejectedObjectionsBefore());

        params.bp.TotalDecisionsOnObjectionsBefore_Rule_Succeeded(result);

        return result;
    },

    A_eq_B_C_D_2nd_round_Rule: function (a, params) {
        if (
            params.bp.FullyAdmittedObjectionsCount == undefined ||
            params.bp.PartiallyAdmittedObjectionsCount == undefined ||
            params.bp.UnfoundedObjectionsCount == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.FullyAdmittedObjectionsCount()) +
            parseInt(params.bp.PartiallyAdmittedObjectionsCount()) +
            parseInt(params.bp.UnfoundedObjectionsCount());

        params.bp.TotalDecisionsOnObjectionsCount_Rule_Succeeded(result);

        return result;
    },

    A_eq_B_C_D_CECE_1_day_1st_round_Rule_Before: function (a, params) {
        if (
            params.bp.DecisionsFullyAdmittingContestationsUntilEDay == undefined ||
            params.bp.DecisionsPartiallyAdmittingContestationsUntilEDay == undefined ||
            params.bp.ContestationsRejectedUntilEDay == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.DecisionsFullyAdmittingContestationsUntilEDay()) +
            parseInt(params.bp.DecisionsPartiallyAdmittingContestationsUntilEDay()) +
            parseInt(params.bp.ContestationsRejectedUntilEDay());

        params.bp.TotalDecisionsOnContestationsUntilEDay_Rule_Succeeded(result);

        return result;
    },

    A_eq_B_C_D_CECE_1_day_1st_round_Rule: function (a, params) {
        if (
            params.bp.DecisionsFullyAdmittingContestationsOnEDay == undefined ||
            params.bp.DecisionsPartiallyAdmittingContestationsOnEDay == undefined ||
            params.bp.ContestationsRejectedOnEDay == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.DecisionsFullyAdmittingContestationsOnEDay()) +
            parseInt(params.bp.DecisionsPartiallyAdmittingContestationsOnEDay()) +
            parseInt(params.bp.ContestationsRejectedOnEDay());

        params.bp.ContestationsOnEDayDecisions_Rule_Succeeded(result);

        return result;
    },

    A_B_C_eq_D_E_F_eq_G_H_Rule: function (a, params) {
        if (
            params.bp.ElectoralCompetitorRequestsCount == undefined ||
            params.bp.ObserverRequestsCount == undefined ||
            params.bp.InclusionRequestsCount == undefined ||
            params.bp.ExclusionRequestsCount == undefined ||
            params.bp.DataCorrectionRequestsCount == undefined ||
            params.bp.AdmittedRequestsCount == undefined ||
            params.bp.RejectedRequestsCount == undefined
        ) {
            return false;
        }

        let result = false;

        if (
            (
                parseInt(params.bp.VoterRequestsCount()) +
                parseInt(params.bp.ElectoralCompetitorRequestsCount()) +
                parseInt(params.bp.ObserverRequestsCount()) ===
                parseInt(params.bp.InclusionRequestsCount()) +
                parseInt(params.bp.ExclusionRequestsCount()) +
                parseInt(params.bp.DataCorrectionRequestsCount())
            ) &&
            (
                parseInt(params.bp.InclusionRequestsCount()) +
                parseInt(params.bp.ExclusionRequestsCount()) +
                parseInt(params.bp.DataCorrectionRequestsCount()) ===
                parseInt(params.bp.AdmittedRequestsCount()) +
                parseInt(params.bp.RejectedRequestsCount())
            )
        ) {
            result = true;
        } else {
            result = false;
        }

        params.bp.VoterRequestsCount_Rule_Succeeded(result);

        return result;
    },

    A_B_C_eq_D_E_F_eq_G_H_2days_Rule: function (a, params) {
        if (
            params.bp.NumberOfRequestsByElectoralRepresentatives == undefined ||
            params.bp.NumberOfRequestsByObservers == undefined ||
            params.bp.NumberOfInclusionRequests == undefined ||
            params.bp.NumberOfExclusionRequests == undefined ||
            params.bp.NumberOfDataCorrectionRequests == undefined ||
            params.bp.NumberOfAdmittedRequests == undefined ||
            params.bp.NumberOfRejectedRequests == undefined
        ) {
            return false;
        }

        let result = false;

        if (
            (parseInt(params.bp.NumberOfRequestsByVoters()) +
                parseInt(params.bp.NumberOfRequestsByElectoralRepresentatives()) +
                parseInt(params.bp.NumberOfRequestsByObservers()) ===
                parseInt(params.bp.NumberOfInclusionRequests()) +
                parseInt(params.bp.NumberOfExclusionRequests()) +
                parseInt(params.bp.NumberOfDataCorrectionRequests())
            ) &&
            parseInt(params.bp.NumberOfInclusionRequests()) +
            parseInt(params.bp.NumberOfExclusionRequests()) +
            parseInt(params.bp.NumberOfDataCorrectionRequests()) ===
            parseInt(params.bp.NumberOfAdmittedRequests()) +
            parseInt(params.bp.NumberOfRejectedRequests())
        ) {
            result = true;
        } else {
            result = false;
        }

        params.bp.NumberOfRequestsByVoters_Rule_Succeeded(result);

        return result;
    },

    A_B_C_eq_D_E_F_eq_G_H_Final_BESV_2days_Rule: function (a, params) {
        if (
            params.bp.NumberOfRequestsByElectoralRepresentatives == undefined ||
            params.bp.NumberOfRequestsByObservers == undefined ||
            params.bp.NumberOfInclusionRequests == undefined ||
            params.bp.NumberOfExclusionRequests == undefined ||
            params.bp.NumberOfDataCorrectionRequests == undefined ||
            params.bp.NumberOfAdmittedRequests == undefined ||
            params.bp.NumberOfRejectedRequests == undefined
        ) {
            return false;
        }

        let result = false;

        if (
            (parseInt(params.bp.NumberOfRequestsByVoters()) +
                parseInt(params.bp.NumberOfRequestsByElectoralRepresentatives()) +
                parseInt(params.bp.NumberOfRequestsByObservers()) ===
                parseInt(params.bp.NumberOfInclusionRequests()) +
                parseInt(params.bp.NumberOfExclusionRequests()) +
                parseInt(params.bp.NumberOfDataCorrectionRequests())
            ) &&
            parseInt(params.bp.NumberOfInclusionRequests()) +
            parseInt(params.bp.NumberOfExclusionRequests()) +
            parseInt(params.bp.NumberOfDataCorrectionRequests()) ===
            parseInt(params.bp.NumberOfAdmittedRequests()) +
            parseInt(params.bp.NumberOfRejectedRequests())
        ) {
            result = true;
        } else {
            result = false;
        }

        params.bp.NumberOfRequestsByVoters_Rule_Succeeded(result);

        return result;
    },

    D_equal_or_less_than_C_Rule: function (d, params) {
        if (params.bp.VotesBasedOnPassportCount == undefined) {
            return false;
        }

        const result = parseInt(d) <= parseInt(params.bp.VotesBasedOnPassportCount());

        params.bp.VotesBasedOnExpiredPassportCount_Rule_Succeeded(result);

        return result;
    },

    D_equal_or_less_than_C_2nd_round_Rule: function (d, params) {
        if (params.bp.VotersWithPassport == undefined) {
            return false;
        }

        const result = parseInt(d) <= parseInt(params.bp.VotersWithPassport());

        params.bp.VotersWithExpiredPassport_Rule_Succeeded(result);

        return result;
    },

    D_equal_or_less_than_C_2days_Rule: function (d, params) {
        if (params.bp.VotersWithPassport == undefined) {
            return false;
        }

        const result = parseInt(d) <= parseInt(params.bp.VotersWithPassport());

        params.bp.VotersWithExpiredPassport_Rule_Succeeded(result);

        return result;
    },

    A_equal_or_more_than_point23A: function (a, params) {
        if (params.bp.SignaturesOnBasicListsFirstDay == undefined) {
            return false;
        }

        const result = parseInt(a) >= parseInt(params.bp.SignaturesOnBasicListsFirstDay());

        params.bp.SignaturesOnBasicListSecondDay_Rule_Succeeded(result);

        return result;
    },

    B_equal_or_more_than_point23B: function (b, params) {
        if (params.bp.SignaturesOnSupplementaryListsFirstDay == undefined) {
            return false;
        }

        const result = parseInt(b) >= parseInt(params.bp.SignaturesOnSupplementaryListsFirstDay());

        params.bp.SignaturesOnSupplementaryListSecondDay_Rule_Succeeded(result);

        return result;
    },

    C_equal_or_more_than_point23C: function (c, params) {
        if (params.bp.SignaturesOnVotingLocationListsFirstDay == undefined) {
            return false;
        }

        const result = parseInt(c) >= parseInt(params.bp.SignaturesOnVotingLocationListsFirstDay());

        params.bp.SignaturesOnVotingLocationListSecondDay_Rule_Succeeded(result);

        return result;
    },

    A_equal_point5_minus_point21_Rule: function (a, params) {
        if (
            params.bp.ElectoralOfficeMembersCompositionCount == undefined ||
            params.bp.AbsentElectoralBoardMembersOnVotingDayCount == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.ElectoralOfficeMembersCompositionCount()) -
            parseInt(params.bp.AbsentElectoralBoardMembersOnVotingDayCount());

        params.bp.ElectoralBoardMembersCount_Rule_Succeeded(result);

        return result;
    },

    A_equal_point5_minus_point21_referendum_Rule: function (a, params) {
        if (
            params.bp.ElectoralBoardMembersNumericComposition == undefined ||
            params.bp.AbsentElectoralBoardMembersOnVotingDay == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.ElectoralBoardMembersNumericComposition()) -
            parseInt(params.bp.AbsentElectoralBoardMembersOnVotingDay());

        params.bp.ElectoralBoardMembersCount_Rule_Succeeded(result);

        return result;
    },

    A_equal_point6_minus_point21_Rule: function (a, params) {
        if (
            params.bp.MeetingsCountBetweenRounds == undefined ||
            params.bp.VotingResultsReportCountByBESV == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.MeetingsCountBetweenRounds()) -
            parseInt(params.bp.VotingResultsReportCountByBESV());

        params.bp.ElectoralBoardMembersCount_Rule_Succeeded(result);

        return result;
    },

    A_equal_point5_minus_point18_Rule: function (a, params) {
        if (
            params.bp.ElectoralBoardMembersNumericComposition == undefined ||
            params.bp.AbsentElectoralBoardMembersOnVotingDay == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.ElectoralBoardMembersNumericComposition()) -
            parseInt(params.bp.AbsentElectoralBoardMembersOnVotingDay());

        params.bp.ElectoralBoardMembersCount_Rule_Succeeded(result);

        return result;
    },

    A_equal_point6_minus_point25_Rule: function (a, params) {
        if (
            params.bp.ElectoralBoardMembersNumericComposition == undefined ||
            params.bp.AbsentElectoralBoardMembersOnVotingDay == undefined
        ) {
            return false;
        }

        const result = parseInt(a) === parseInt(params.bp.ElectoralBoardMembersNumericComposition()) -
            parseInt(params.bp.AbsentElectoralBoardMembersOnVotingDay());

        params.bp.ElectoralBoardMembersCount_Rule_Succeeded(result);

        return result;
    },

    equal_or_less_than_point_5_Rule: function (inputValue, params) {
        if (params.bp.ElectoralOfficeMembersCompositionCount == undefined) {
            return false;
        }

        const result = parseInt(inputValue) <= parseInt(params.bp.ElectoralOfficeMembersCompositionCount());

        params.bp.AbsentElectoralBoardMembersOnVotingDayCount_Rule_Succeeded(result);

        return result;
    },

    equal_or_less_than_point_6_2days_Rule: function (inputValue, params) {
        if (params.bp.ElectoralBoardMembersNumericComposition == undefined) {
            return false;
        }

        const result = parseInt(inputValue) <= parseInt(params.bp.ElectoralBoardMembersNumericComposition());

        params.bp.AbsentElectoralBoardMembersOnVotingDay_Rule_Succeeded(result);

        return result;
    },

    equal_or_less_than_point_6_2days_2nd_day_Rule: function (inputValue, params) {
        if (params.bp.ElectoralBoardMembersNumericComposition == undefined) {
            return false;
        }

        const result = parseInt(inputValue) <= parseInt(params.bp.ElectoralBoardMembersNumericComposition());

        params.bp.AbsentElectoralBoardMembersOnSecondDay_Rule_Succeeded(result);

        return result;
    },

    equal_or_less_than_point_6_Final_BESV_2days_2nd_day_Rule: function (inputValue, params) {
        if (params.bp.ElectoralBoardMembersNumericComposition == undefined) {
            return false;
        }

        const result = parseInt(inputValue) <= parseInt(params.bp.ElectoralBoardMembersNumericComposition());

        params.bp.AbsentElectoralBoardMembersOnVotingDay_Rule_Succeeded(result);

        return result;
    },

    equal_or_less_than_point_6_Final_BESV_2nd_round_Rule: function (inputValue, params) {
        if (params.bp.ElectoralBoardMembersNumericComposition == undefined) {
            return false;
        }

        const result = parseInt(inputValue) <= parseInt(params.bp.ElectoralBoardMembersNumericComposition());

        params.bp.AbsentElectoralBoardMembersOnSecondDay_Rule_Succeeded(result);

        return result;
    },

    equal_or_less_than_point_6_2nd_round_Rule: function (inputValue, params) {
        if (params.bp.MeetingsCountBetweenRounds == undefined) {
            return false;
        }

        const result = parseInt(inputValue) <= parseInt(params.bp.MeetingsCountBetweenRounds());

        params.bp.AbsentElectoralBoardMembersOnSecondDay_Rule_Succeeded(result);

        return result;
    },

    equal_or_less_than_point_6_final_BESV_1day_2nd_round_Rule: function (inputValue, params) {
        if (params.bp.ElectoralBoardMembersNumericComposition == undefined) {
            return false;
        }

        const result = parseInt(inputValue) <= parseInt(params.bp.ElectoralBoardMembersNumericComposition());

        params.bp.AbsentElectoralBoardMembersOnVotingDay_Rule_Succeeded(result);

        return result;
    },

    equal_or_less_than_point_5_referendum_1day_Rule: function (inputValue, params) {
        if (params.bp.ElectoralBoardMembersNumericComposition == undefined) {
            return false;
        }

        const result = parseInt(inputValue) <= parseInt(params.bp.ElectoralBoardMembersNumericComposition());

        params.bp.AbsentElectoralBoardMembersOnVotingDay_Rule_Succeeded(result);

        return result;
    },

    qual_or_less_than_point_1_Rule: function (inputValue, params) {
        if (params.bp.CouncilMembers == undefined) {
            return false;
        }

        const result = parseInt(inputValue) <= parseInt(params.bp.CouncilMembers());

        params.bp.AbsentCouncilMembers_Rule_Succeeded(result);

        return result;
    },

    equal_or_less_than_point_1_CECE_2days_Rule: function (inputValue, params) {
        if (params.bp.CouncilMembers == undefined) {
            return false;
        }

        const result = parseInt(inputValue) <= parseInt(params.bp.CouncilMembers());

        params.bp.CouncilMembersAbsentOnEDay_Rule_Succeeded(result);

        return result;
    },

    equal_or_less_than_point_1_CECE_2days_Rule_2nd_day: function (inputValue, params) {
        if (params.bp.CouncilMembers == undefined) {
            return false;
        }

        const result = parseInt(inputValue) <= parseInt(params.bp.CouncilMembers());

        params.bp.CouncilMembersAbsentOnDay2OfVoting_Rule_Succeeded(result);

        return result;
    },

    C_GT_A_B_Rule: function (c, params) {
        if (params.bp.RegisteredVoters == undefined || params.bp.Supplementary == undefined) {
            return false;
        }

        var result = parseInt(c) <= (parseInt(params.bp.RegisteredVoters()) + parseInt(params.bp.Supplementary()));
        params.bp.BallotsIssued_Rule1_Succeeded(result);
        return result;
    },
    C_LT_D_Rule: function (c, params) {
        if (params.bp.BallotsCasted == undefined) {
            return false;
        }

        var result = parseInt(c) >= parseInt(params.bp.BallotsCasted());
        params.bp.BallotsIssued_Rule2_Succeeded(result);
        return result;
    },
    E_NE_C_D_Rule: function (e, params) {
        if (params.bp.BallotsIssued == undefined || params.bp.BallotsCasted == undefined) {
            return false;
        }

        var result = parseInt(e) === (parseInt(params.bp.BallotsIssued()) - parseInt(params.bp.BallotsCasted()));
        return result;
    },

    F_eq_D_minus_H_Rule: function (f, params) {
        if (params.bp.BallotsCasted == undefined || params.bp.BallotsValidVotes == undefined) {
            return false;
        }

        var result = parseInt(f) === (parseInt(params.bp.BallotsCasted()) - parseInt(params.bp.BallotsValidVotes()));

        return result;
    },

    D_eq_F_H_Rule: function (d, params) {
        if (params.f == undefined || params.h == undefined) {
            return false;
        }

        var result = parseInt(d) === (parseInt(params.f()) + parseInt(params.h()));
        return result;
    },
    H_eq_Sum_of_g_Rule: function (h, params) {
        var sumOfCompetitors = 0;

        if (params.competitorsArray == undefined) {
            return false;
        }

        $.map(params.competitorsArray(), function (c, i) {
            sumOfCompetitors += parseInt(c.BallotCount());
        });

        var result = (parseInt(h) === sumOfCompetitors);
        return result;
    },
    I_eq_C_J_Rule: function (i, params) {
        if (params.c == undefined || params.j == undefined) {
            return false;
        }

        var result = parseInt(i) === (parseInt(params.c()) + parseInt(params.j()));
        return result;
    },

    J_eq_I_diff_C_Rule: function (j, params) {
        if (params.bp.BallotsIssued == undefined || params.bp.BallotsReceived == undefined) {
            return false;
        }

        var result = parseInt(j) === (parseInt(params.bp.BallotsReceived()) - parseInt(params.bp.BallotsIssued()));

        return result;
    },

    X_GT_Zero_Rule: function (x) {
        var result = parseInt(x) > 0;
        return result;
    },

    gt_Zero_IfLocal: function (x, params) {
        var val = parseInt(x);
        var result = (params.isLocal && !params.allowZero) ? val > 0 : val >= 0;
        return result;
    }
}