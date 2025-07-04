locals {
  namespace         = "votemonitor-${var.env}"
  availability_zone = data.aws_availability_zones.current.names[0]

  images = {
    api = {
      image = "commitglobal/votemonitor"
      tag   = "0.2.64"
    }

    hangfire = {
      image = "commitglobal/votemonitor-hangfire"
      tag   = "0.2.64"
    }
  }

  ecs = {
    instance_types = {
      # "t3a.medium" = ""
      "m5a.large" = ""
    }
  }

  db = {
    name           = "votemonitor"
    instance_class = var.env == "production" ? "db.t4g.small" : "db.t4g.micro"
  }

  networking = {
    cidr_block = "10.0.0.0/16"

    public_subnets = [
      "10.0.1.0/24",
      "10.0.2.0/24",
      "10.0.3.0/24"
    ]

    private_subnets = [
      "10.0.4.0/24",
      "10.0.5.0/24",
      "10.0.6.0/24"
    ]
  }
}
