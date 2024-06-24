﻿const customStyles = `
            .template-page-container {
                padding: 20px 198px;
                color: black;
            }

            .template-container {
                background-color: white;
                padding: 120px 30px 120px 90px;
                /*padding: 0 60px;*/
                font-family: 'Times New Roman', Times, serif;
                font-size: 16px;
                line-height: 1.2;
                /*width: 720px;*/
            }

            p {
                margin: 0 0 10px;
            }

            .template-container p {
                margin: 0;
            }

            .template-header {
                padding: 0 36px;
                text-align: center;
            }

            .template-header .top-section {
                display: flex;
                justify-content: space-between;
            }

            p.election-state-organ {
                font-weight: bold;
                margin-bottom: 36px;
            }

            .election-description {
                margin-bottom: 0;
            }

            .circumscription-region,
            p.circumscription-location,
            .document-name,
            .explanation-info-block,
            .formulas-verification-block {
                margin-bottom: 40px;
            }

            p.circumscription-location {
                font-weight: bold;
            }

            .document-name {
                font-weight: bold;
            }

            .electoral-statistics-grid,
            .candidate-votes-grid {
                display: grid;
                grid-template-columns: 60px 4fr 1fr;
                margin-bottom: 32px;
            }

            .electoral-statistics-grid div,
            .candidate-votes-grid div {
                border: 1px solid black;
                padding: 4px;
            }

            .electoral-statistics-grid div:not(:last-child),
            .candidate-votes-grid div:not(:last-child) {
                border-bottom: none;
            }

            .electoral-statistics-grid div:nth-last-child(2),
            .electoral-statistics-grid div:nth-last-child(3),
            .candidate-votes-grid div:nth-last-child(2),
            .candidate-votes-grid div:nth-last-child(3) {
                border: 1px solid black;
            }

            .electoral-statistics-grid div:not(.electoral-statistic-item-value),
            .candidate-votes-grid div:not(.candidate-vote-item-value) {
                border-right: none;
            }

            .electoral-statistics-grid div:not(.electoral-statistic-item-name),
            .candidate-votes-grid div:not(.candidate-vote-item-name){
                position: relative;
            }

            .electoral-statistics-grid div:not(.electoral-statistic-item-name) p,
            .candidate-votes-grid div:not(.candidate-vote-item-name) p {
                position: absolute;
                transform: translate(-50%, -50%);
                top: 50%;
                left: 50%;
            }

            .candidate-votes-grid div.candidate-vote-item-value.column-header p {
                position: static;
                transform: unset;
            }

            .candidate-votes-grid .candidate-vote-item-name.column-header {
                text-align: center;
            }

            .electoral-statistic-item-letter,
            .electoral-statistic-item-value,
            .candidate-vote-item-letter:not(.column-header),
            .candidate-vote-item-value:not(.column-header) {
                font-weight: bold;
            }

            .electoral-statistic-item-letter,
            .electoral-statistic-item-value,
            .candidate-vote-item-letter,
            .candidate-vote-item-value {
                text-align: center;
            }

            .candidate-votes-grid.column-headers {
                margin-bottom: 0;
            }

            .candidate-vote-item-name,
            .circumscription-region,
            .circumscription-region-without-name,
            .election-description {
                text-transform: uppercase;
            }

            .candidate-votes-grid.column-headers div {
                border-bottom: none;
                text-transform: unset;
            }

            .candidate-votes-grid.column-headers div.candidate-vote-item-name.column-header {
                position: relative;
            }

            .candidate-votes-grid.column-headers div.candidate-vote-item-name.column-header p {
                position: absolute;
                transform: translateY(-50%);
                top: 50%;
            }

            .explanation-info-block {
                padding: 0 36px;
                font-size: 15px;
            }

            .explanation-info-block p:last-child {
                font-size: 14px;
            }

            .formulas-verification-block {
                padding: 0 80px;
            }

            .candidate-votes-grid {
                margin-bottom: 100px;
            }

            .presidential-signatures {
                display: grid;
                grid-template-columns: 3fr 7fr 1fr 3fr;
                margin-bottom: 60px;
            }

            .presidential-signatures-grid-item,
            .electoral-members-signatures-grid-item {
                margin-right: 16px;
            }

            .presidential-signatures-grid-item.signature,
            .presidential-signatures-grid-item.signature-column-header,
            .electoral-members-signatures-grid-item.signature,
            .electoral-members-signatures-grid-item.signature-column-header,
            .electoral-members-signatures-grid-item.empty {
                margin-right: 0;
            }

            .presidential-signatures-grid-item.name,
            .presidential-signatures-grid-item.signature,
            .electoral-members-signatures-grid-item.name,
            .electoral-members-signatures-grid-item.signature {
                border-bottom: 1px solid black;
            }

            .presidential-signatures-grid-item.gender,
            .electoral-members-signatures-grid-item.gender {
                border: 1px solid black;
            }

            .presidential-signatures-grid-item:nth-child(11) {
                border-top: none;
            }

            .electoral-members-signatures-grid-item:nth-child(7) {
                border-top: 1px solid black;
            }

            .presidential-signatures-grid-item.column-header,
            .electoral-members-signatures-grid-item.column-header,
            .electoral-members-signatures-title {
                text-align: center;
            }

            .electoral-members-signatures {
                display: grid;
                grid-template-columns: 3fr 7fr 1fr 3fr;
                margin-bottom: 60px;
            }

            .electoral-members-signatures-grid-item:not(.column-header),
            .presidential-signatures-grid-item:not(.column-header) {
                height: 24px;
            }

            .electoral-members-signatures-grid-item.gender {
                border-top: none;
            }

            .electoral-members-signatures-grid-item:nth-child(7) {
                border-top: 1px solid black;
            }

            p.electoral-members-signatures-title {
                margin-bottom: 12px
            }

            .print-template-footer {
                display: grid;
                grid-template-columns: 1fr 1fr;
                gap: 16px;
                padding-left: 64px;
            }

            .footer-grid-item.date,
            .footer-grid-item.time {
                justify-self: end;
            }

            .footer-grid-item:not(.date),
            .footer-grid-item:not(.time) {
                text-align: center;
            }

            .footer-grid-item .day-placeholder {
                margin-right: 12px;
            }

            .president-polling-station {
                display: grid;
                grid-template-columns: 214px 120px 180px;
                column-gap: 58px;
                margin: 40px 0;
            }

            .president-polling-station .signature-placeholder,
            .president-polling-station .name-placeholder {
                border-bottom: 1px solid black;
            }

            .president-polling-station .signature,
            .president-polling-station .name {
                text-align: center;
                font-style: italic;
            }

            .president-polling-station .president {
                margin-right: 18px;
            }

            .president-polling-station .signature,
            .president-polling-station .signature-placeholder {
                margin-right: 32px;
            }

            .print-template-footer.report-final-besv {
                padding-left: 0;
                grid-template-columns: 253px auto;
            }

            .print-template-footer.report-final-besv .footer-grid-item:first-child {
                text-align: unset;
            }

            .template-page-container li,
            .template-page-container p.report-group-title,
            .template-page-container p.report-intro {
                margin-bottom: 12px;
            }

            p.report-group-title span.report-group-item,
            p.document-type {
                font-weight: bold;
            }

            p.document-type {
                margin-bottom: 40px;
            }

            .election-validity {
                margin-bottom: 8px
            }

            p.mayor-chosen-intro {
                margin-bottom: 16px;
            }

            p.election-validity-placeholder {
                display: flex;
                justify-content: end;
                margin-right: 66px;
            }

            .mayor-chosen-grid {
                display: grid;
                grid-template-columns: 1fr 1fr;
                margin-bottom: 16px;
            }

            .mayor-chosen-grid-item,
            .candidates-for-second-round-grid-item {
                border: 1px solid black;
                height: 48px;
                padding: 4px 0;
            }

            .mayor-chosen-grid-item.mayor-chosen-name,
            .mayor-chosen-grid-item.block-or-party {
                border-top: none;
            }

            .mayor-chosen-grid-item.mayor-chosen-name,
            .mayor-chosen-grid-item:first-child {
                border-right: none;
            }

            .candidates-for-second-round-grid-item:first-child {
                font-weight: bold;
            }

            .candidates-for-second-round-grid-item:nth-child(1),
            .candidates-for-second-round-grid-item:nth-child(2),
            .candidates-for-second-round-grid-item:nth-child(3),
            .candidates-for-second-round-grid-item:nth-child(4),
            .candidates-for-second-round-grid-item:nth-child(5),
            .candidates-for-second-round-grid-item:nth-child(6) {
                border-bottom: none;
            }

            .candidates-for-second-round-grid-item:nth-child(1),
            .candidates-for-second-round-grid-item:nth-child(2),
            .candidates-for-second-round-grid-item:nth-child(4),
            .candidates-for-second-round-grid-item:nth-child(5),
            .candidates-for-second-round-grid-item:nth-child(7),
            .candidates-for-second-round-grid-item:nth-child(8) {
                border-right: none;
            }

            .mayor-chosen-grid-item p,
            .candidates-for-second-round-grid-item p {
                margin: 0;
                text-align: center;
            }

            .candidates-for-second-round {
                display: grid;
                grid-template-columns: 48px 1fr 1fr;
                margin-bottom: 40px;
            }

            .document-name {
                margin: 40px 0;
            }

            p.referendum-question {
                margin-bottom: 40px;
            }

            .wider-column {
                width: 160px;
            }

            .circumscription-region p:first-child {
                margin-bottom: 8px;
            }

            @media print {
                p {
                    margin: 0 0 10px;
                }

                #sidebar-left,
                #printTemplate,
                header.navbar {
                    display: none !important;
                }

                .template-page-container {
                    padding: 0;
                    margin-top: 0;
                    font-size: 12pt;
                }

                .template-container {
                    padding: 0;
                    margin-top: 0;
                }

                .template-container2,
                .template-container3,
                .template-container4,
                .template-container5,
                .template-container6,
                .template-container7 {
                    padding-top: 8px !important;
                }

                .container-fluid {
                    margin-top: 0;
                }

                ul li {
                    list-style: none;
                }

                .wider-column {
                    width: 160px;
                }

                .circumscription-region p:first-child {
                    margin-bottom: 8px;
                }
            }
        `;