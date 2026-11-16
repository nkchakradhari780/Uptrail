import 'package:flutter/material.dart';
import 'package:uptrail/app/router/app_router.dart';
import 'package:uptrail/app/theme/app_theme.dart';

class EmployeeStatusApp extends StatelessWidget {
  const EmployeeStatusApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp.router(
      title: 'Employee Status',
      debugShowCheckedModeBanner: false,
      theme: AppTheme.lightTheme,
      routerConfig: appRouter,
    );
  }
}