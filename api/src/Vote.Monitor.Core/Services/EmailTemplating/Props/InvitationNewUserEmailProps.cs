﻿namespace Vote.Monitor.Core.Services.EmailTemplating.Props;

public record InvitationNewUserEmailProps(string CdnUrl, string AcceptUrl) : BaseEmailProps(CdnUrl);
