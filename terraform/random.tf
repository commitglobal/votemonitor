resource "random_string" "secrets_suffix" {
  length  = 8
  special = false
  upper   = false
  numeric = false

  lifecycle {
    ignore_changes = [
      length,
      special,
      upper,
      numeric,
    ]
  }
}

# JWT_SIGNING_KEY
resource "random_password" "jwt_signing_key" {
  length  = 64
  special = false

  lifecycle {
    ignore_changes = [
      length,
      special
    ]
  }
}

resource "aws_secretsmanager_secret" "jwt_signing_key" {
  name = "${local.namespace}-jwt_signing_key-${random_string.secrets_suffix.result}"
}

resource "aws_secretsmanager_secret_version" "jwt_signing_key" {
  secret_id     = aws_secretsmanager_secret.jwt_signing_key.id
  secret_string = random_password.jwt_signing_key.result
}


# Seed admin
resource "random_password" "seed_admin_password" {
  length  = 20
  special = false

  lifecycle {
    ignore_changes = [
      length,
      special
    ]
  }
}


resource "aws_secretsmanager_secret" "seed_admin" {
  name = "${local.namespace}-seed_admin-${random_string.secrets_suffix.result}"
}


resource "aws_secretsmanager_secret_version" "seed_admin" {
  secret_id = aws_secretsmanager_secret.seed_admin.id
  secret_string = jsonencode({
    firstname = "Admini"
    lastname  = "Strator"
    email     = var.seed_admin_email
    phone     = var.seed_admin_phone
    password  = random_password.seed_admin_password.result
  })
}

resource "aws_secretsmanager_secret" "ses_staging" {
 name = "${local.namespace}-ses-staging-${random_string.secrets_suffix.result}"
}
