module "ecs_hangfire" {
  source = "./modules/ecs-service"

  depends_on = [
    module.ecs_cluster
  ]

  name         = "hangfire-${var.env}"
  cluster_name = module.ecs_cluster.cluster_name
  min_capacity = 1
  max_capacity = 1

  image_repo = local.images.hangfire.image
  image_tag  = local.images.hangfire.tag

  use_load_balancer       = var.use_load_balancer
  lb_dns_name             = aws_lb.main.dns_name
  lb_zone_id              = aws_lb.main.zone_id
  lb_vpc_id               = aws_vpc.main.id
  lb_listener_arn         = aws_lb_listener.https.arn
  lb_hosts                = ["hangfire.${var.domain_name}"]
  lb_domain_zone_id       = data.aws_route53_zone.main.zone_id
  lb_health_check_enabled = true
  lb_path                 = "/health"

  container_memory_soft_limit = 256
  container_memory_hard_limit = 768

  log_group_name                 = module.ecs_cluster.log_group_name
  service_discovery_namespace_id = module.ecs_cluster.service_discovery_namespace_id

  container_port          = 80
  network_mode            = "awsvpc"
  network_security_groups = [aws_security_group.ecs.id]
  network_subnets         = [aws_subnet.private.0.id]

  task_role_arn          = aws_iam_role.ecs_task_role.arn
  enable_execute_command = var.enable_execute_command

  # predefined_metric_type = "ECSServiceAverageCPUUtilization"
  # target_value           = 65

  ordered_placement_strategy = [
    {
      type  = "spread"
      field = "instanceId"
    },
    {
      type  = "binpack"
      field = "memory"
    }
  ]

  environment = [
    {
      name  = "ASPNETCORE_ENVIRONMENT"
      value = var.env
    },
    {
      name  = "FileStorage__FileStorageType"
      value = "S3"
    },
    {
      name  = "ASPNETCORE_URLS",
      value = "http://+:80"
    },
    {
      name  = "FileStorage__S3__BucketName"
      value = module.s3_private.bucket
    },
    {
      name  = "FileStorage__S3__AWSRegion"
      value = var.region
    },
    {
      name  = "Mailing__MailSenderType"
      value = "SES"
    },
    {
      name  = "Mailing__SES__SenderName"
      value = "VoteMonitor"
    },
    {
      name  = "Mailing__SES__SenderEmail"
      value = "no-reply@${var.domain_name}"
    },
    {
      name  = "Mailing__SES__AWSRegion"
      value = var.region
    },
    {
      name  = "PushNotifications__SenderType"
      value = "Expo"
    },
  ]

  secrets = [
    {
      name      = "Domain__DbConnectionConfig__Server"
      valueFrom = "${aws_secretsmanager_secret.rds.arn}:host::"
    },
    {
      name      = "Domain__DbConnectionConfig__Port"
      valueFrom = "${aws_secretsmanager_secret.rds.arn}:port::"
    },
    {
      name      = "Domain__DbConnectionConfig__Database"
      valueFrom = "${aws_secretsmanager_secret.rds.arn}:database::"
    },
    {
      name      = "Domain__DbConnectionConfig__UserId"
      valueFrom = "${aws_secretsmanager_secret.rds.arn}:username::"
    },
    {
      name      = "Domain__DbConnectionConfig__Password"
      valueFrom = "${aws_secretsmanager_secret.rds.arn}:password::"
    },
    {
      name      = "Core__HangfireConnectionConfig__Server"
      valueFrom = "${aws_secretsmanager_secret.rds.arn}:host::"
    },
    {
      name      = "Core__HangfireConnectionConfig__Port"
      valueFrom = "${aws_secretsmanager_secret.rds.arn}:port::"
    },
    {
      name      = "Core__HangfireConnectionConfig__Database"
      valueFrom = "${aws_secretsmanager_secret.rds.arn}:database::"
    },
    {
      name      = "Core__HangfireConnectionConfig__UserId"
      valueFrom = "${aws_secretsmanager_secret.rds.arn}:username::"
    },
    {
      name      = "Core__HangfireConnectionConfig__Password"
      valueFrom = "${aws_secretsmanager_secret.rds.arn}:password::"
    },
    {
      name      = "DashboardAuth__Username"
      valueFrom = "${aws_secretsmanager_secret.seed_admin.arn}:email::"
    },
    {
      name      = "DashboardAuth__Password"
      valueFrom = "${aws_secretsmanager_secret.seed_admin.arn}:password::"
    },
    {
      name      = "Mailing__SES__AWSAccessKey"
      valueFrom = "${aws_secretsmanager_secret.ses_hotfix.arn}:access_key::"
    },
    {
      name      = "Mailing__SES__AWSSecretKey"
      valueFrom = "${aws_secretsmanager_secret.ses_hotfix.arn}:secret_key::"
    },
        {
      name      = "FileStorage__S3__AWSSecretKey"
      valueFrom = "${aws_secretsmanager_secret.ses_hotfix.arn}:access_key::"
    },
    {
      name      = "FileStorage__S3__AWSAccessKey"
      valueFrom = "${aws_secretsmanager_secret.ses_hotfix.arn}:secret_key::"
    },
  ]

  allowed_secrets = [
    aws_secretsmanager_secret.seed_admin.arn,
    aws_secretsmanager_secret.rds.arn,
    aws_secretsmanager_secret.ses_hotfix.arn,
  ]
}
