terraform {
  required_version = "~> 1.17"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.100"
    }
  }

  cloud {
    hostname     = "terrakube-api.serval-mark.ts.net"
    organization = "commitglobal"

    workspaces {
      tags = [
        "votemonitor"
      ]
    }
  }
}

provider "aws" {
  region = var.region

  default_tags {
    tags = {
      app = "votemonitor"
      env = var.env
    }
  }
}
