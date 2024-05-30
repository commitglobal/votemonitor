resource "aws_iam_user" "ses_hotfix" {
  name = "${local.namespace}-ses-user"
}

resource "aws_iam_access_key" "ses_hotfix" {
  user = aws_iam_user.ses_hotfix.name
}

resource "aws_secretsmanager_secret" "ses_hotfix" {
  name = "${local.namespace}-ses_hotfix-${random_string.secrets_suffix.result}"
}

resource "aws_secretsmanager_secret_version" "ses_hotfix" {
  secret_id = aws_secretsmanager_secret.ses_hotfix.id
  secret_string = jsonencode({
    access_key = aws_iam_access_key.ses_hotfix.id,
    secret_key = aws_iam_access_key.ses_hotfix.secret
  })
}

data "aws_iam_policy_document" "ses_hotfix" {
  statement {
    actions = [
      "ses:SendEmail",
      "ses:SendRawEmail",
    ]

    resources = [
      aws_sesv2_email_identity.main.arn,
      aws_sesv2_configuration_set.main.arn,
    ]
  }

  statement {
    actions = [
      "s3:ListBucket",
      "s3:GetObject",
      "s3:DeleteObject",
      "s3:GetObjectAcl",
      "s3:PutObjectAcl",
      "s3:PutObject",
      "s3:ListMultipartUploadParts",
    ]

    resources = [
      module.s3_private.arn,
      "${module.s3_private.arn}/*"
    ]
  }
}

resource "aws_iam_user_policy" "ses_hotfix" {
  name   = "ses_hotfix"
  user   = aws_iam_user.ses_hotfix.name
  policy = data.aws_iam_policy_document.ses_hotfix.json
}
